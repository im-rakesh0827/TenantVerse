using System.Net.Http.Json;
using Blazored.LocalStorage;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Authentication.Requests;
using TenantVerse.Shared.Models.Authentication.Responses;

namespace TenantVerse.UI.Services.Authentication;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;

    private const string BaseUrl = "http://localhost:5168/api/Auth";
    private const string TokenKey = "token";

    public AuthService(
        HttpClient httpClient,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
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

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();

        if (result is not null &&
            result.IsSuccess &&
            !string.IsNullOrWhiteSpace(result.Data?.Token))
        {
            await _localStorage.SetItemAsync(TokenKey, result.Data.Token);
        }

        return result;
    }

    #endregion

    #region Logout

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
    }

    #endregion

    #region Token

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>(TokenKey);
    }

    public async Task<bool> IsLoggedInAsync()
    {
        var token = await GetTokenAsync();

        return !string.IsNullOrWhiteSpace(token);
    }

    #endregion
}