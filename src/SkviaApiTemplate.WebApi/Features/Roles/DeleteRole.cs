using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Roles;

public class DeleteRole : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{id}", Handle)
            .WithPermission(Permissions.Roles.Delete)
            .WithSummary("Eliminar rol")
            .WithDescription("Elimina un rol por su ID")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

    private static async Task<IResult> Handle(
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var affectedRows = await db.Roles
            .Where(r => r.Id == id)
            .ExecuteDeleteAsync(ct);

        return affectedRows == 0 ? 
            IdentityErrors.RoleNotFound(id).ToProblem() :
            Results.NoContent();
    }
        
}
