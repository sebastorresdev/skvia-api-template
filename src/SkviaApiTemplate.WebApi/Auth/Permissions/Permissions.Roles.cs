using System.ComponentModel;

namespace SkviaApiTemplate.WebApi.Auth.Permissions;

public static partial class Permissions
{
    public static class Roles
    {
        [Description("Ver Roles")]
        public const string View = "Permissions.Roles.View";

        [Description("Crear Rol")]
        public const string Create = "Permissions.Roles.Create";

        [Description("Editar Rol")]
        public const string Edit = "Permissions.Roles.Edit";

        [Description("Eliminar Rol")]
        public const string Delete = "Permissions.Roles.Delete";
    }
}