using SkviaApiTemplate.WebApi.Auth.Permissions;
using SkviaApiTemplate.WebApi.Auth.Services;

namespace SkviaApiTemplate.WebApi.Features.Users;

public class CreateUser : IEndpoint
{
    public record CreateUserRequest(
        string Name,
        string Username,
        string Password,
        string? ProfilePicture,
        string? Email,
        string? PhoneNumber,
        Guid RoleId);

    private record CreateUserResponse(Guid Id, string UserName);

    public class CreateUserValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es requerido.");
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("El nombre de usuario es requerido.")
                .MinimumLength(3).WithMessage("El nombre de usuario debe tener mínimo 3 caracteres.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("La contraseña es requerida.")
                .MinimumLength(6).WithMessage("La contraseña debe tener mínimo 6 caracteres.");
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("El rol es requerido.");
        }
    }

    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/", Handle)
            .WithRequestValidation<CreateUserRequest>()
            .WithPermission(Permissions.Users.Create)
            .WithSummary("Crear usuario")
            .WithDescription("Crea un nuevo usuario en el sistema.")
            .Produces<CreateUserResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);
            
    private static async Task<IResult> Handle(
        CreateUserRequest request,
        AppDbContext db,
        IPasswordService passwordService,
        CancellationToken ct)
    {
        var normalizedUserName = request.Username.Trim().ToUpperInvariant();

        if (await db.Users.AnyAsync(u => u.NormalizedUsername == normalizedUserName, ct))
            return IdentityErrors.DuplicateUser(request.Username).ToProblem();

        if (!await db.Roles.AnyAsync(r => r.Id == request.RoleId, ct))
            return IdentityErrors.RoleNotFound(request.RoleId).ToProblem();

        var passwordHash = passwordService.Hash(request.Password);
        
        var user = User.Create(
            name : request.Name,
            username : request.Username,
            passwordHash : passwordHash,
            roleId : request.RoleId,
            email : request.Email,
            phoneNumber : request.PhoneNumber,
            profilePicture : request.ProfilePicture);

        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created(
            $"/api/usuarios/{user.Id}",
            new CreateUserResponse(user.Id, user.Username));
    }
}