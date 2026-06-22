using SkviaApiTemplate.WebApi.Auth.Security;
using SkviaApiTemplate.WebApi.Auth.Services;

namespace SkviaApiTemplate.WebApi.Features.Auth;

public class Login : IEndpoint
{
    public record LoginRequest(string UserName, string Password);
    private record LoginResponse(string Token, string UserName);

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/login", Handle)
             .AllowAnonymous()
             .WithSummary("Iniciar sesión")
             .WithDescription("Autentica al usuario y retorna el token JWT.")
             .Produces<LoginResponse>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> Handle(
        LoginRequest request,
        AppDbContext db,
        IJwtService jwtService,
        IPasswordService passwordService,
        CancellationToken ct)
    {
        var normalizedUserName = request.UserName.Trim().ToUpperInvariant();

        var user = await db.Users
            .AsNoTracking()
            .Where(u => u.NormalizedUsername == normalizedUserName)
            .Select(u => new
            {
                Id = u.Id.ToString(),
                UserName = u.Username,
                PasswordHash = u.PasswordHash,
                RoleName = u.Role.Name,
                Permissions = u.Role.RolePermissions
                    .Select(rp => rp.Permission.Code)
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if(user is null || !passwordService.Verify(request.Password, user.PasswordHash))
            return IdentityErrors.InvalidCredentials.ToProblem();

        var token = jwtService.GenerateToken(
            user.Id,
            user.UserName,
            user.RoleName,
            user.Permissions);

        return TypedResults.Ok(new LoginResponse(token, user.UserName));
    }
}
