using System.Net.Http.Json;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Authentication.Requests;
using TenantVerse.Shared.Models.Authentication.Responses;

namespace TenantVerse.UI.Services.Authentication;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private string baseUrl = "http://localhost:5168/api/Auth";

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<RegisterResponse>?> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/register", request);

        return await response.Content.ReadFromJsonAsync<ApiResponse<RegisterResponse>>();
    }

     public async Task<ApiResponse<LoginResponse>?> LoginAsync(LoginRequest request)
     {
     var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/login", request);

     return await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
     }
}