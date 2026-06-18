using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Users;

public class UpdateUserStatus : IEndpoint
{
    public record UpdateUserStatusRequest(bool IsActive);

    public static void Map(RouteGroupBuilder group)
        => group.MapPatch("/{id:guid}/status", Handle)
            .WithPermission(Permissions.Users.Edit)
            .WithSummary("Cambiar estado de usuario")
            .WithDescription("Archiva o desarchiva a un usuario en el sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        Guid id,
        UpdateUserStatusRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var user = await db.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null)
            return IdentityErrors.UserNotFound(id).ToProblem();

        if (request.IsActive)
            user.Unarchive();
        else 
            user.Archive();

        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}