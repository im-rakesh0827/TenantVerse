using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TenantVerse.UI.Services.Authentication;

public static class JwtHelper
{
    public static IEnumerable<Claim> GetClaims(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Enumerable.Empty<Claim>();

        var handler = new JwtSecurityTokenHandler();

        if (!handler.CanReadToken(token))
            return Enumerable.Empty<Claim>();

        var jwt = handler.ReadJwtToken(token);

        return jwt.Claims;
    }
}