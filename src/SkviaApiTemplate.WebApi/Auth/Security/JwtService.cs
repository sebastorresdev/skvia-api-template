using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace SkviaApiTemplate.WebApi.Auth.Security;

public interface IJwtService
{
    string GenerateToken(
        string userId,
        string userName,
        string roleName,
        List<string> permission);
}

public class JwtSettings
{
    public const string Section = nameof(JwtSettings);

    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpiryMinutes { get; set; }
}
public class JwtService(IOptions<JwtSettings> options) : IJwtService
{
    public string GenerateToken(
        string userId,
        string userName,
        string roleName,
        List<string> permissions)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new Dictionary<string, object>
        {
            { JwtRegisteredClaimNames.Sub, userId },
            { JwtRegisteredClaimNames.UniqueName, userName },
            { ClaimTypes.Role, roleName },
            { CustomClaimTypes.Permissions,  permissions }
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = options.Value.Issuer,
            Audience = options.Value.Audience,
            Claims = claims,
            Expires = DateTime.UtcNow.AddMinutes(options.Value.ExpiryMinutes),
            SigningCredentials = credentials
        };

        var tokenHandler = new JsonWebTokenHandler();

        return tokenHandler.CreateToken(tokenDescriptor);
    }
}