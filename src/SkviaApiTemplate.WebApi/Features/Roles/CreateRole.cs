using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Roles;

public class CreateRole : IEndpoint
{
    public record CreateRoleRequest(string Name, string? Description);
    private record CreateRoleResponse(Guid Id, string RoleName);
    
    public class CreateRoleValidator : AbstractValidator<CreateRoleRequest>
    {
        public CreateRoleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del rol es requerido.")
                .MinimumLength(3).WithMessage("El nombre del rol debe tener mínimo 3 caracteres.");
        }
    }

    public static void Map(RouteGroupBuilder group)
        => group.MapPost("", Handle)
            .WithRequestValidation<CreateRoleRequest>()
            .WithPermission(Permissions.Roles.Create)
            .WithSummary("Crear Rol")
            .WithDescription("Crea un nuevo rol en el sistema")
            .Produces<CreateRoleResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        CreateRoleRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var normalizedRoleName = request.Name.ToUpperInvariant();
        var roleExists = await db.Roles
            .AnyAsync(r => r.NormalizedName == normalizedRoleName, ct);

        if(roleExists)
            return IdentityErrors.DuplicateRole(request.Name).ToProblem();

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
        };

        db.Roles.Add(role);
        await db.SaveChangesAsync(ct);

        return TypedResults.Created(
            $"/api/roles/{role.Id}", new CreateRoleResponse(role.Id, role.Name));
    }
}
