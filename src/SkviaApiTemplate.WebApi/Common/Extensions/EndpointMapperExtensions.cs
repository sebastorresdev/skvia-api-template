using SkviaApiTemplate.WebApi.Common.Filters;
using System.Reflection;

namespace SkviaApiTemplate.WebApi.Common.Extensions;

public static class EndpointMapperExtensions
{
    extension(WebApplication app)
    {
        public WebApplication MapEndpoints(Assembly assembly)
        {
            var endpointTypes = assembly.DefinedTypes
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                     && t.IsAssignableTo(typeof(IEndpoint)));

            var grouped = endpointTypes.GroupBy(t =>
            {
                var ns = t.Namespace ?? "";
                var segments = ns.Split('.');
                var idx = Array.IndexOf(segments, "Features");
                return segments[idx + 1].ToLower();
            });

            foreach(var group in grouped)
            {
                var routeGroup = app.MapGroup($"/api/{group.Key}")
                    .WithTags(group.Key)
                    .RequireAuthorization()
                    .AddEndpointFilter<PermissionFilter>()
                    .ProducesProblem(StatusCodes.Status403Forbidden);

                foreach(var type in group)
                    type.GetMethod("Map")!.Invoke(null, [routeGroup]);
            }

            return app;
        }
    }
}
