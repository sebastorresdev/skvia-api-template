using SkviaApiTemplate.WebApi.Auth.Security;
using System.Security.Claims;

namespace SkviaApiTemplate.WebApi.Auth;

public class CurrentUser(IHttpContextAccessor accessor)
{
    private readonly ClaimsPrincipal? _user = accessor.HttpContext?.User;

    public Guid GetId()
    {
        var idValue = _user?.FindFirstValue(ClaimTypes.NameIdentifier);
        return idValue != null ? Guid.Parse(idValue) : Guid.Empty;
    }

    public string GetUserName() =>
        _user?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public bool HasPermission(string permission) =>
        _user?.Claims.Any(c => c.Type == CustomClaimTypes.Permissions &&
            string.Equals(c.Value, permission, StringComparison.OrdinalIgnoreCase)) ?? false;

    public bool IsAdmin() =>
        _user?.IsInRole(Roles.Administrador) ?? false;
}
