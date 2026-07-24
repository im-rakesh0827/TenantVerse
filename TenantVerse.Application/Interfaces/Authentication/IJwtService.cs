using TenantVerse.Shared.Models.Authentication;

namespace TenantVerse.Application.Interfaces.Authentication;

public interface IJwtService
{
    string GenerateToken(UserLoginData user);
}