using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Branches;

public class DeleteBranch
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{id:guid}", Handle)
            .WithPermission(Permissions.Branches.Delete)
            .WithSummary("Eliminar sede")
            .WithDescription("Elimina una sede existente por su ID.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(Guid id, AppDbContext db, CancellationToken cancellationToken)
    {
        var affectedRows = await db.Branches
            .Where(u => u.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return affectedRows == 0 ? 
            IdentityErrors.UserNotFound(id).ToProblem() : 
            TypedResults.NoContent();
    }
}