using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Roles;

public class GetRoleById : IEndpoint
{
    private record RoleDetailResponse(
        Guid Id,
        string Name,
        string? Description,
        List<PermissionResponse> Permissions);

    private record PermissionResponse(
        Guid Id,
        string Name,
        string? Description);

    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{id:guid}", Handle)
            .WithPermission(Permissions.Roles.View)
            .WithSummary("Obtener rol por ID")
            .WithDescription("Obtiene los detalles de un rol específico por su ID, incluyendo sus permisos asociados.")
            .Produces<RoleDetailResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);


    private static async Task<IResult> Handle(
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var roleDetailResponse = await db.Roles
        .AsNoTracking()
        .Where(r => r.Id == id)
        .Select(r => new RoleDetailResponse(
            Id: r.Id,
            Name: r.Name,
            Description: r.Description,
            Permissions: r.RolePermissions.Select(rp => new PermissionResponse(
                rp.Permission.Id,
                rp.Permission.Name,
                rp.Permission.Description
            )).ToList()
        ))
        .FirstOrDefaultAsync(ct);

        return roleDetailResponse is null ? 
            IdentityErrors.RoleNotFound(id).ToProblem() : 
            TypedResults.Ok(roleDetailResponse);
    }
}
