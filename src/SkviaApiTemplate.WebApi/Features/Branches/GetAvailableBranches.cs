using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Branches;

public class GetAvailableBranches : IEndpoint
{
    private record BranchResponse(
        Guid Id,
        string Name,
        string? Address);

    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/available", Handle) 
            .WithPermission(Permissions.Branches.View)
            .WithSummary("Obtener tiendas disponibles")
            .WithDescription("Retorna la lista de tiendas activas para asignación o filtrado.")
            .Produces<List<BranchResponse>>(StatusCodes.Status200OK);

    private static async Task<IResult> Handle(
        AppDbContext db,
        CancellationToken ct)
    {
        var branches = await db.Branches
            .AsNoTracking()
            .Where(b => b.IsActive && !db.Users.Any(u => u.BranchId == b.Id))
            .Select(b => new BranchResponse(
                Id: b.Id,
                Name: b.Name,
                Address: b.Address))
            .ToListAsync(ct);

        return TypedResults.Ok(branches);
    }
}