using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Branches;

public class GetBranches : IEndpoint
{
    private record BranchResponse(
        Guid Id,
        string Name,
        string? Address);

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithPermission(Permissions.Branches.View)
            .WithSummary("Obtener sedes")
            .WithDescription("Retorna todas las sedes del sistema.")
            .Produces<List<BranchResponse>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> Handle(
        AppDbContext db,
        CancellationToken ct)
    {
        var branches = await db.Branches
            .AsNoTracking()
            .Select(b => new BranchResponse(
                Id: b.Id,
                Name: b.Name,
                Address: b.Address))
            .ToListAsync(ct);

        return TypedResults.Ok(branches);
    }
}