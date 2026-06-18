using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Features.Roles;

public class UpdateRole : IEndpoint
{
    public record UpdateRoleRequest(string Name, string? Description);

    public class UpdateRoleValidator : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del rol es requerido.")
                .MinimumLength(3).WithMessage("El nombre del rol debe tener mínimo 3 caracteres.");
        }
    }

    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}", Handle)
            .WithRequestValidation<UpdateRoleRequest>()
            .WithPermission(Permissions.Roles.Edit)
            .WithSummary("Editar rol")
            .WithDescription("Edita un rol existente en el sistema.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        UpdateRoleRequest request,
        AppDbContext db,
        CancellationToken ct)
    {
        var rol = await db.Roles.FindAsync([id], ct);

        if (rol is null)
            return IdentityErrors.RoleNotFound(id).ToProblem();

        var normalizedRoleName = request.Name.Trim().ToUpperInvariant();

        var roleExists = await db.Roles
            .AnyAsync(r => r.Id != id && r.NormalizedName == normalizedRoleName, ct);

        if(roleExists)
            return IdentityErrors.DuplicateRole(request.Name).ToProblem();

        rol.Name = request.Name;
        rol.Description = request.Description;
        
        await db.SaveChangesAsync(ct);
        return TypedResults.NoContent();
    }
}
