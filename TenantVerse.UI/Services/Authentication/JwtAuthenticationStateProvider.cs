using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TenantVerse.UI.Services.Authentication;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly TokenService _tokenService;

    private ClaimsPrincipal _currentUser =
        new(new ClaimsIdentity());

    public JwtAuthenticationStateProvider(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(
            new AuthenticationState(_currentUser));
    }

    public async Task InitializeAsync()
    {
        var token = await _tokenService.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
            return;

        var claims = JwtHelper.GetClaims(token);

        MarkUserAsAuthenticated(claims);
    }

    public void MarkUserAsAuthenticated(IEnumerable<Claim> claims)
    {
        var identity = new ClaimsIdentity(claims, "jwt");

        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public void MarkUserAsLoggedOut()
    {
        _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser)));
    }
}