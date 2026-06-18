namespace SkviaApiTemplate.WebApi.Auth.Security;

// 💡 Con esto le decimos a .NET que esta etiqueta se puede poner en métodos (endpoints) y clases (Commands/Queries)
[AttributeUsage(AttributeTargets.Method)]
public sealed class HasPermissionAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}