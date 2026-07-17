using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace RetailSphere.UI.Configuration.Authentication;

/// <summary>
/// Mirrors the JwtAuthenticationStateProvider pattern used in Platform.UI: the
/// access token lives in memory (not local storage — XSS mitigation) for the
/// lifetime of the app session, while the refresh token is persisted so a page
/// reload doesn't force a re-login. JwtTokenMessageHandler reads the in-memory
/// access token on every outgoing request and calls RefreshAsync on a 401.
/// </summary>
public sealed class JwtAuthenticationStateProvider(ILocalStorageService localStorage) : AuthenticationStateProvider
{
    private const string RefreshTokenStorageKey = "retailsphere_refresh_token";

    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public string? AccessToken { get; private set; }

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
            return Task.FromResult(Anonymous);

        var claims = new JwtSecurityTokenHandler().ReadJwtToken(AccessToken).Claims;
        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
    }

    public async Task SetSessionAsync(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        await localStorage.SetItemAsStringAsync(RefreshTokenStorageKey, refreshToken);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void SetAccessToken(string accessToken)
    {
        AccessToken = accessToken;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task<string?> GetStoredRefreshTokenAsync() =>
        await localStorage.GetItemAsStringAsync(RefreshTokenStorageKey);

    public async Task LogoutAsync()
    {
        AccessToken = null;
        await localStorage.RemoveItemAsync(RefreshTokenStorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }
}
