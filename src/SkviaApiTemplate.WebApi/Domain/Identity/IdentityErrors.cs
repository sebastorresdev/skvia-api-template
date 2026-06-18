namespace SkviaApiTemplate.WebApi.Domain.Identity;

public static class IdentityErrors
{
    // ERRORES DE AUTENTICACIÓN
    public static Error InvalidCredentials =>
       Error.Unauthorized(
           code: "Login.InvalidCredentials",
           description: "El nombre de usuario o la contraseña son incorrectos.");

    // ERRORES DE USUARIOS
    public static Error DuplicateUser(string userName) =>
        Error.Conflict(
            code: "User.DuplicateUser",
            description: $"El nombre de usuario '{userName}' ya está en uso.");

    public static Error UserNotFound(Guid id) =>
        Error.NotFound(
            code: "User.UserNotFound",
            description: $"No se encontró un usuario con el ID '{id}'.");
    
    // ERRORES DE ROLES
    public static Error RoleNotFound(Guid id)
        => Error.NotFound(
            code: "Role.NotFound",
             description: $"No se encontró el rol con ID '{id}'.");

    public static Error DuplicateRole(string roleName) =>
        Error.Conflict(
            code: "Role.DuplicateRole",
            description: $"El nombre del rol '{roleName}' ya está en uso.");
}
