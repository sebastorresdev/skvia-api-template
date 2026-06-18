using SkviaApiTemplate.WebApi.Auth.Permissions;
using SkviaApiTemplate.WebApi.Domain.Entities;
using SkviaApiTemplate.WebApi.Domain.Errors;

namespace SkviaApiTemplate.WebApi.Features.Branches;

public class CreateBranch : IEndpoint
{
    public record CreateBranchRequest(string Name, string? Address);

    public class CreateBranchValidator : AbstractValidator<CreateBranchRequest>
    {
        public CreateBranchValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
        }
    }

    private record CreateBranchResponse(Guid Id);

    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithRequestValidation<CreateBranchRequest>()
            .WithPermission(Permissions.Branches.Create)
            .WithSummary("Crear sucursal")
            .WithDescription("Crea una nueva tienda/sucursal en el sistema.")
            .Produces<CreateBranchResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        CreateBranchRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var cleanName = request.Name.Trim();
        
        var branchExisting = await db.Branches
            .AnyAsync(b => b.Name.ToLower() == cleanName.ToLower(), ct);

        if (branchExisting) 
            return BranchesErrors.DuplicateBranch(request.Name).ToProblem();

        var branch = Branch.Create(
            name: request.Name,
            address: request.Address);

        db.Branches.Add(branch);
        await db.SaveChangesAsync(ct);
        
        return TypedResults.Created($"/branches/{branch.Id}", 
            new CreateBranchResponse(branch.Id));
    }
}