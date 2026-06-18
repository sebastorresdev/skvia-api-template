using SkviaApiTemplate.WebApi.Auth.Permissions;
using SkviaApiTemplate.WebApi.Auth.Services;

namespace SkviaApiTemplate.WebApi.Features.Users;

public class ChangeUserPassword : IEndpoint
{
    public record ChangeUserPasswordRequest(string NewPassword);

    public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordRequest>
    {
        public ChangeUserPasswordValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("La contraseña debe contener como minimo 6 caracteres");
        }
    }

    public static void Map(RouteGroupBuilder group)
        => group.MapPatch("/{id}/password", Handle)
            .WithRequestValidation<ChangeUserPasswordRequest>()
            .WithPermission(Permissions.Users.Edit)
            .WithSummary("Cambiar contraseña")
            .WithDescription("Permite cambiar la contraseña");

    private static async Task<IResult> Handle(
        Guid id,
        ChangeUserPasswordRequest request,
        PasswordService passwordService,
        AppDbContext db,
        CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null) return IdentityErrors.UserNotFound(id).ToProblem();

        var newPasswordHash = passwordService.Hash(request.NewPassword);

        user.ChangePassword(newPasswordHash);

        await db.SaveChangesAsync(ct);

        return TypedResults.NoContent();
    }
}