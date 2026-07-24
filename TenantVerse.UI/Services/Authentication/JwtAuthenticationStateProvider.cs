using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace TenantVerse.UI.Services.Authentication;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous =
        new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        return Task.FromResult(
            new AuthenticationState(_anonymous));
    }

    public void MarkUserAsAuthenticated(string email)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.Name, email)
        ], "jwt");

        var user = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(user)));
    }

    public void MarkUserAsLoggedOut()
    {
        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(_anonymous)));
    }
}