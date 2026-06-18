using System.ComponentModel; // 🌟 OBLIGATORIO para usar [Description]

namespace SkviaApiTemplate.WebApi.Auth.Permissions;

public static partial class Permissions
{
    public static class Users
    {
        [Description("Ver Usuarios")] // 👈 Este texto se guardará en el campo 'Name' de la BD
        public const string View = "Permissions.Users.View"; // 👈 Código limpio en inglés para tus Minimal APIs

        [Description("Crear Usuario")]
        public const string Create = "Permissions.Users.Create";

        [Description("Editar Usuario")]
        public const string Edit = "Permissions.Users.Edit";

        [Description("Eliminar Usuario")]
        public const string Delete = "Permissions.Users.Delete";
    }
}