using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Authentication.Requests;
using TenantVerse.Shared.Models.Authentication.Responses;

namespace TenantVerse.Application.Interfaces.Authentication;

public interface IAuthService
{
    Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request);

    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
}