namespace SkviaApiTemplate.WebApi.Domain.Common;

public interface IEntity
{
}

public interface IEntity<out T> : IEntity
{
    T Id { get; }
}