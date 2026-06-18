using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Users;

public class GetUserById : IEndpoint
{
    private record UserDetailResponse(
        Guid Id,
        string Name,
        string Username,
        string? ProfilePicture,
        string? PhoneNumber,
        string? Email,
        Guid RoleId,
        bool IsActive);
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{id:guid}", Handle)
            .WithPermission(Permissions.Users.View)
            .WithSummary("Obtener usuario por ID")
            .WithDescription("Retorna los detalles de un usuario específico por su ID.")
            .Produces<UserDetailResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        AppDbContext db,
        CancellationToken ct)
    {
        var user = await db.Users
            .AsNoTracking()
            .Where(u => u.Id == id)
            .Select(u => new UserDetailResponse(
                Id: u.Id,
                Name: u.Name,
                Username: u.Username,
                ProfilePicture: u.ProfilePicture,
                PhoneNumber: u.PhoneNumber,
                Email: u.Email,
                RoleId: u.RoleId,
                IsActive: u.IsActive))
            .FirstOrDefaultAsync(ct);

        return user is not null
            ? TypedResults.Ok(user)
            : TypedResults.NotFound();
    }
}
