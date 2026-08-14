using System.Net.Http.Json;
using Blazored.LocalStorage;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Authentication.Requests;
using TenantVerse.Shared.Models.Authentication.Responses;
using Blazored.LocalStorage;
namespace TenantVerse.UI.Services.Authentication;

public class AuthService
{
    private readonly HttpClient _httpClient;
    // private readonly ILocalStorageService _localStorage;
    private readonly JwtAuthenticationStateProvider _authenticationStateProvider;
    private readonly TokenService _tokenService;
    private const string BaseUrl = "http://localhost:5168/api/Auth";
    private const string TokenKey = "token";

    public AuthService(
    HttpClient httpClient,
    TokenService tokenService,
    JwtAuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _authenticationStateProvider = authenticationStateProvider;
    }

    #region Register

    public async Task<ApiResponse<RegisterResponse>?> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/register", request);

        return await response.Content.ReadFromJsonAsync<ApiResponse<RegisterResponse>>();
    }

    #endregion

    #region Login

    public async Task<ApiResponse<LoginResponse>?> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/login", request);
        var result =
        await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
        if (result is not null &&
            result.IsSuccess &&
            !string.IsNullOrWhiteSpace(result.Data?.Token))
        {
            await _tokenService.SaveTokenAsync(result.Data.Token);
            var claims = JwtHelper.GetClaims(result.Data.Token);
            _authenticationStateProvider.MarkUserAsAuthenticated(claims);
        }
        return result;
    }

    #endregion

    #region Logout

    public async Task LogoutAsync()
    {
        await _tokenService.RemoveTokenAsync();
        _authenticationStateProvider.MarkUserAsLoggedOut();
    }

    #endregion

    #region Token

    public async Task<string?> GetTokenAsync()
    {
        return await _tokenService.GetTokenAsync();
    }

    public async Task<bool> IsLoggedInAsync()
    {
        var token = await GetTokenAsync();

        return !string.IsNullOrWhiteSpace(token);
    }

    #endregion
}