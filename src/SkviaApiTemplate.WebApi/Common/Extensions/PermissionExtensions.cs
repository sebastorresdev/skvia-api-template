using SkviaApiTemplate.WebApi.Auth.Security;

namespace SkviaApiTemplate.WebApi.Common.Extensions;

public static class PermissionExtensions
{
    extension(RouteHandlerBuilder builder)
    {
        public RouteHandlerBuilder WithPermission(string permiso)
            => builder.WithMetadata(new HasPermissionAttribute(permiso));
    }
}
