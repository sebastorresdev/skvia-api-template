using SkviaApiTemplate.WebApi.Domain.Common;

namespace SkviaApiTemplate.WebApi.Domain.Entities;

public class Branch : BaseAuditableEntity
{
    public required string Name
    {
        get;
        set
        {
            var cleanValue = value?.Trim() ?? string.Empty;
            field = cleanValue;
            NormalizedName = cleanValue.ToUpperInvariant();
        }
    }
    
    public string NormalizedName { get; private set; } = null!;
    public string? Address { get; private set; }
    
    private Branch() {}
    
    public static Branch Create(string name, string? address = null)
    {
        return new Branch
        {
            Name = name.Trim(),
            Address = address?.Trim()
        };
    }

    public void Update(string name, string? address = null)
    {
        Name = name;
        Address = address;
    }
}