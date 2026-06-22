using SkviaApiTemplate.WebApi.Auth.Permissions;
using SkviaApiTemplate.WebApi.Domain.Entities;
using SkviaApiTemplate.WebApi.Domain.Errors;

namespace SkviaApiTemplate.WebApi.Features.Branches;

public class CreateBranch : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithSummary("Crear sucursal")
            .WithRequestValidation<CreateBranchRequest>()
            .WithPermission(Permissions.Branches.Create)
            .Produces<CreateBranchResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        CreateBranchRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var cleanCode = request.Code.Trim().ToUpperInvariant();
        
        var branchExisting = await db.Branches
            .AnyAsync(b => b.Code == cleanCode, ct);

        if (branchExisting) 
            return BranchesErrors.DuplicateBranch(request.Code).ToProblem();

        var branch = Branch.Create(
            code: request.Code,
            name: request.Name,
            address: request.Address);

        db.Branches.Add(branch);
        await db.SaveChangesAsync(ct);
        
        return TypedResults.Created($"/branches/{branch.Id}", 
            new CreateBranchResponse(branch.Id));
    }
}

public record CreateBranchRequest(string Code, string Name, string? Address);

public class CreateBranchValidator : AbstractValidator<CreateBranchRequest>
{
    public CreateBranchValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters.");
    }
}

public record CreateBranchResponse(Guid Id);