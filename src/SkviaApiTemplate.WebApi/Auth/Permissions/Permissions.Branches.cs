using System.ComponentModel;

namespace SkviaApiTemplate.WebApi.Auth.Permissions;

public static partial class Permissions
{
    public static class Branches
    {
        [Description("Crear Sucursal")]
        public const string Create = "Permissions.Branches.Create";

        [Description("Actualizar Sucursal")]
        public const string Update = "Permissions.Branches.Update";

        [Description("Eliminar Sucursal")]
        public const string Delete = "Permissions.Branches.Delete";

        [Description("Ver Sucursales")]
        public const string View = "Permissions.Branches.View";
    }
}