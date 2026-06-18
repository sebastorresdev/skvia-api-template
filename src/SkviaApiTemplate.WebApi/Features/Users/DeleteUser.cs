using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Users;

public class DeleteUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{id:guid}", Handle)
            .WithPermission(Permissions.Users.Delete)
            .WithSummary("Eliminar usuario")
            .WithDescription("Elimina un usuario existente por su ID.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(Guid id, AppDbContext db, CancellationToken cancellationToken)
    {
        var affectedRows = await db.Users
                .Where(u => u.Id == id)
                .ExecuteDeleteAsync(cancellationToken);

        return affectedRows == 0 ? 
            IdentityErrors.UserNotFound(id).ToProblem() : 
            TypedResults.NoContent();
    }
}
