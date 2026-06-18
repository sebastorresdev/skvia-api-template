using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Roles;

public class GetRoles : IEndpoint
{
    private record RoleResponse(
        Guid Id,
        string Name);
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithPermission(Permissions.Roles.View)
            .WithSummary("Obtener todos los roles")
            .WithDescription("Obtiene una lista de todos los roles disponibles en el sistema.")
            .Produces<List<RoleResponse>>(StatusCodes.Status200OK);

    private static async Task<IResult> Handle(
        AppDbContext db,
        CancellationToken ct)
    {
        var roles = await db.Roles
            .AsNoTracking()
            .Select(r => new RoleResponse(
                Id: r.Id,
                Name: r.Name
            ))
            .ToListAsync(ct);
        return TypedResults.Ok(roles);
    }
}
