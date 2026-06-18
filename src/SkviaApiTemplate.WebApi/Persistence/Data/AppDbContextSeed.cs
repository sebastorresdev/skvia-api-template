using System.ComponentModel;
using System.Reflection;
using SkviaApiTemplate.WebApi.Auth.Services;
using SkviaApiTemplate.WebApi.Auth.Permissions;

namespace SkviaApiTemplate.WebApi.Persistence.Data;

public static class AppDbContextSeed
{
    public static async Task SeedAsync(AppDbContext db)
    {
        // 1. Descubrir permisos dinámicamente mediante Reflexión leyendo los atributos [Description]
        var permissionsToSeed = GetPermissionsFromConstants();

        // Obtener códigos existentes en la base de datos para no duplicar nada si el seeder corre de nuevo
        var existingPermissionCodes = await db.Permissions
            .Select(p => p.Code)
            .ToListAsync();

        // Filtrar solo los que son verdaderamente nuevos
        var newPermissions = permissionsToSeed
            .Where(p => !existingPermissionCodes.Contains(p.Code))
            .ToList();

        if (newPermissions.Count != 0)
        {
            db.Permissions.AddRange(newPermissions);
            await db.SaveChangesAsync(); 
        }

        // Solo sembramos el Rol y el Usuario Administrador por primera vez si no existen usuarios
        if (!await db.Users.AnyAsync())
        {
            // Volver a obtener todos los permisos para asegurar que tenemos los IDs correctos
            var allPermissionsInDb = await db.Permissions.ToListAsync();

            // 2. Crear Rol Administrador y asignarle absolutamente todos los permisos
            var roleAdmin = new Role { Name = "Administrador" };
            roleAdmin.AssignPermissions(allPermissionsInDb.Select(p => p.Id).ToList());

            db.Roles.Add(roleAdmin);
            await db.SaveChangesAsync(); // Guardamos el rol para obtener su Id

            // 3. Crear Usuario Administrador maestro de SkviaApiTemplate
            var adminUser = User.Create(
                name: "Sebastian David Torres Chavez",
                username: "storres",
                passwordHash: new PasswordService().Hash("gal22v10"),
                roleId: roleAdmin.Id,
                email: "storres@bubbabag.com" // Opcional pero recomendado
            );

            db.Users.Add(adminUser);
            await db.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Extrae de forma dinámica todas las constantes de permisos, lee su atributo [Description] 
    /// y mapea los objetos listos para impactar en la base de datos.
    /// </summary>
    private static List<Permission> GetPermissionsFromConstants()
    {
        var permissions = new List<Permission>();
        
        // Obtiene todas las clases anidadas (Branches, Users, etc.) dentro de Permissions
        var modules = typeof(Permissions).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var module in modules)
        {
            // Filtra solo los campos constantes de tipo string
            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

            foreach (var fi in fields)
            {
                var codeValue = fi.GetRawConstantValue()?.ToString();
                if (string.IsNullOrEmpty(codeValue)) continue;

                // 🧠 MAGIA: Buscamos si la constante tiene el atributo [Description] encima
                var descriptionAttribute = fi.GetCustomAttribute<DescriptionAttribute>();
                
                // Si lo tiene usa el texto amigable (ej: "Crear Sucursal"), si no, usa el nombre técnico del campo
                var amigableName = descriptionAttribute?.Description ?? fi.Name;

                permissions.Add(new Permission
                {
                    Code = codeValue, // Tu set nativo de la entidad se encargará de rellenar el NormalizedCode solo 🚀
                    Name = amigableName,
                    Description = $"Permite realizar la acción de {amigableName.ToLower()} en el sistema."
                });
            }
        }
        
        return permissions;
    }
}