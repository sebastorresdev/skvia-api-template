using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Branches;

public class GetBranchById : IEndpoint
{
    private record BranchResponse(
        Guid Id,
        string Name,
        string? Address );
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{id:guid}", Handle)
            .WithPermission(Permissions.Branches.View)
            .WithSummary("Obtener sede por ID")
            .WithDescription("Retorna los detalles de una sede específica por su ID.")
            .Produces<BranchResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var branch = await db.Branches
            .AsNoTracking()
            .Where(b => b.Id == id)
            .Select(b => new BranchResponse(
                Id: b.Id,
                Name: b.Name,
                Address: b.Address))
            .FirstOrDefaultAsync(ct);

        return branch is not null
            ? TypedResults.Ok(branch)
            : TypedResults.NotFound();
    }
}