namespace SkviaApiTemplate.WebApi.Common.Endpoints;

public interface IEndpoint
{
    static abstract void Map(RouteGroupBuilder group);
}