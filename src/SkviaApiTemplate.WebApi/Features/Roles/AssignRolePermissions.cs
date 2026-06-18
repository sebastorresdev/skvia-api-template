using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Roles;

public class AssignRolePermissions : IEndpoint
{
    public record AssignRolePermissionsRequest(List<Guid> PermissionIds);

    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}/permisos", Handle)
            .WithPermission(Permissions.Roles.Edit)
            .WithSummary("Asigna permisos a un rol")
            .WithDescription("Asigna una lista de permisos a un rol específico, reemplazando los permisos actuales.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        AssignRolePermissionsRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var role = await db.Roles
            .Include(r => r.RolePermissions) 
            .FirstOrDefaultAsync(r => r.Id == id, ct);

        if(role is null) return TypedResults.NotFound();

        var validPermissionIds = await db.Permissions
            .Where(p => request.PermissionIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync(ct);
        
        if(validPermissionIds.Count != request.PermissionIds.Count)
            return TypedResults.BadRequest("Algunos permisos no existen.");

        role.AssignPermissions(validPermissionIds);
        await db.SaveChangesAsync(ct);
        
        return TypedResults.NoContent();
    }
}