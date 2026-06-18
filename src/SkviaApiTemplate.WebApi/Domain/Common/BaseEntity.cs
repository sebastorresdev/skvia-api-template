namespace SkviaApiTemplate.WebApi.Domain.Common;

public abstract class BaseEntity : IEntity<Guid>
{
    public Guid Id { get; private set; } = Guid.NewGuid();
}
