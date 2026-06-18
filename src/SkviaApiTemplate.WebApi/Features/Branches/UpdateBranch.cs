using SkviaApiTemplate.WebApi.Auth.Permissions;
using SkviaApiTemplate.WebApi.Domain.Errors;

namespace SkviaApiTemplate.WebApi.Features.Branches;

public class UpdateBranch : IEndpoint
{
    public record UpdateBranchRequest(string Name, string? Address);

    public class UpdateBranchValidator : AbstractValidator<UpdateBranchRequest>
    {
        public UpdateBranchValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
        }
    }

    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}", Handle)
            .WithRequestValidation<UpdateBranchRequest>()
            .WithPermission(Permissions.Branches.Update)
            .WithSummary("Actualizar sucursal")
            .WithDescription("Modifica los datos de una tienda/sucursal existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid id,
        UpdateBranchRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var branch = await db.Branches.FirstOrDefaultAsync(b => b.Id == id, ct);

        if (branch is null)
            return BranchesErrors.NotFound(id).ToProblem();
        
        var cleanNormalizedName = request.Name.ToUpperInvariant();
        
        if(await db.Branches
               .AnyAsync(b => b.NormalizedName == cleanNormalizedName && b.Id != id, ct))
            return BranchesErrors.DuplicateBranch(request.Name).ToProblem();

        branch.Update(
            name: request.Name,
            address: request.Address);

        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}