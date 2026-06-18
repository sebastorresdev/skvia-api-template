using SkviaApiTemplate.WebApi.Auth;
using SkviaApiTemplate.WebApi.Auth.Security;

namespace SkviaApiTemplate.WebApi.Common.Filters;

public class PermissionFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        if(endpoint is null) return await next(context);

        var attributePermission = endpoint.Metadata.GetMetadata<HasPermissionAttribute>();
        if(attributePermission is null) return await next(context);

        // 💡 Inyectamos tu servicio actualizado
        var currentUserService = context.HttpContext.RequestServices.GetRequiredService<CurrentUser>();

        if(currentUserService.GetId() == Guid.Empty)
            return Results.Unauthorized();

        // 💡 Validación directa, semántica y súper limpia
        if(!currentUserService.HasPermission(attributePermission.Permission))
            return Results.Forbid();

        return await next(context);
    }
}