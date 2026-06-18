using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Users;

public class GetUsers : IEndpoint
{
    private record UserResponse(
        Guid Id,
        string Name,
        string Username,
        string? ProfilePicture,
        string? Email,
        string RoleName,
        bool IsActive);

    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", Handle)
            .WithPermission(Permissions.Users.View)
            .WithSummary("Obtener usuarios")
            .WithDescription("Retorna todos los usuarios del sistema.")
            .Produces<List<UserResponse>>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> Handle(
        AppDbContext db,
        CancellationToken ct)
    {
        var users = await db.Users
            .AsNoTracking()
            .Select(u => new UserResponse(
                Id: u.Id,
                Name: u.Name,
                Username: u.Username,
                ProfilePicture: u.ProfilePicture,
                Email: u.Email,
                RoleName: u.Role.Name,
                IsActive: u.IsActive))
            .ToListAsync(ct);

        return TypedResults.Ok(users);
    }
}