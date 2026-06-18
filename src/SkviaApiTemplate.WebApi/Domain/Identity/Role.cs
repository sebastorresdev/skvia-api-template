using SkviaApiTemplate.WebApi.Domain.Common;

namespace SkviaApiTemplate.WebApi.Domain.Identity;

public class Role : BaseAuditableEntity
{
    public required string Name
    {
        get;
        set
        {
            var cleanValue = value?.Trim() ?? null!;
            field = cleanValue;
            NormalizedName = cleanValue.ToUpperInvariant();
        }
    }
    public string NormalizedName { get; private set; } = null!;
    public string? Description { get; set; }

    public ICollection<User> Users { get; private set; } = [];
    public ICollection<RolePermission> RolePermissions { get; private set; } = [];

    public void AssignPermissions(IEnumerable<Guid> permissionIds)
    {
        RolePermissions.Clear();
        foreach(var permissionId in permissionIds)
            RolePermissions.Add(new RolePermission { RoleId = Id, PermissionId = permissionId });
    }
}
