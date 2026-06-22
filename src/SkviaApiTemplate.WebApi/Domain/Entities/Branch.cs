using SkviaApiTemplate.WebApi.Domain.Common;

namespace SkviaApiTemplate.WebApi.Domain.Entities;

public class Branch : BaseAuditableEntity
{
    public required string Code
    {
        get;
        set => field = value?.Trim().ToUpperInvariant() ?? string.Empty;
    }
    

    public string Name { get; private set; } = null!;
    public string? Address { get; private set; }
    
    private Branch() {}
    
    public static Branch Create(string code, string name, string? address = null)
    {
        return new Branch
        {
            Code = code,
            Name = name.Trim(),
            Address = address?.Trim()
        };
    }

    public void Update(string code, string name, string? address = null)
    {
        Code = code;
        Name = name.Trim();
        Address = address?.Trim();
    }
}