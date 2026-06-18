using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Users;

public class UpdateUser : IEndpoint
{
    public record UpdateUserRequest(
        string Name,
        string Username,
        string? ProfilePicture,
        string? Email,
        string? PhoneNumber,
        Guid RoleId);

    public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.");
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                .MinimumLength(3).WithMessage("El nombre de usuario debe tener mínimo 3 caracteres.");
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("El correo electrónico no tiene un formato válido.")
                .MaximumLength(100).WithMessage("El correo electrónico no puede exceder los 100 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.Email));
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("El rol es requerido.");
        }
    }

    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", Handle)
            .WithRequestValidation<UpdateUserRequest>()
            .WithPermission(Permissions.Users.Edit)
            .WithSummary("Editar usuario")
            .WithDescription("Modifica los datos de un usuario existente.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> Handle(
        Guid id,
        UpdateUserRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

        if (user is null)
            return IdentityErrors.UserNotFound(id).ToProblem();

        var normalizedUserName = request.Username.Trim().ToUpperInvariant();

        if (await db.Users.AnyAsync(u => u.NormalizedUsername == normalizedUserName && u.Id != id, ct))
            return IdentityErrors.DuplicateUser(request.Username).ToProblem();

        if (!await db.Roles.AnyAsync(r => r.Id == request.RoleId, ct))
            return IdentityErrors.RoleNotFound(request.RoleId).ToProblem();

        user.Update(
            name: request.Name,
            username: request.Username,
            roleId: request.RoleId,
            email: request.Email,
            profilePicture: request.ProfilePicture,
            phoneNumber: request.PhoneNumber);
        
        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}