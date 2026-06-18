using SkviaApiTemplate.WebApi.Domain.Common;

namespace SkviaApiTemplate.WebApi.Domain.Identity;

public class Permission : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string Code
    {
        get;
        set
        {
            var cleanValue = value?.Trim() ?? string.Empty!;
            field = cleanValue;
            NormalizedCode = cleanValue.ToUpperInvariant();
        }
    }
    public string NormalizedCode { get; private set; } = string.Empty!;

    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
