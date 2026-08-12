using Blazored.LocalStorage;

namespace TenantVerse.UI.Services.Authentication;

public class TokenService
{
    private readonly ILocalStorageService _localStorage;

    private const string TokenKey = "token";

    public TokenService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task SaveTokenAsync(string token)
    {
        await _localStorage.SetItemAsync(TokenKey, token);
    }

    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsync<string>(TokenKey);
    }

    public async Task RemoveTokenAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
    }

    public async Task<bool> HasValidTokenAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrWhiteSpace(token);
    }
}