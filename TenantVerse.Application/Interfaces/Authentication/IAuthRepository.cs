using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Authentication.Requests;
using TenantVerse.Shared.Models.Authentication.Responses;
using TenantVerse.Shared.Models.Authentication;

namespace TenantVerse.Application.Interfaces.Authentication;

public interface IAuthRepository
{
    Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);
    Task<UserLoginData?> GetUserByEmailAsync(string email);
}