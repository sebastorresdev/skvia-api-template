namespace SkviaApiTemplate.WebApi.Domain.Common;

public interface IAuditableEntity
{
    DateTimeOffset CreatedAt { get; set; }
    Guid CreatedBy { get; set; }
    
    DateTimeOffset? UpdatedAt { get; set; }
    Guid? UpdatedBy { get; set; }
    
    bool IsActive { get; }
}

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public Guid CreatedBy { get; set; }
    
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    public bool IsActive { get; private set; } = true;
    
    public void Archive() 
        => IsActive = false;

    public void Unarchive() 
        => IsActive = true;
}