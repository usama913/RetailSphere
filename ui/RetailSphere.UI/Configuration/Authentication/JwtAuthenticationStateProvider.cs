using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Http;
using RetailSphere.Contracts.Auth;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Configuration.Authentication;

/// <summary>
/// Mirrors the JwtAuthenticationStateProvider pattern used in Platform.UI: the
/// access token lives in memory (not local storage — XSS mitigation) for the
/// lifetime of the app session, while the refresh token is persisted so a page
/// reload doesn't force a re-login. JwtTokenMessageHandler reads the in-memory
/// access token on every outgoing request and calls RefreshAsync on a 401.
///
/// A full browser reload restarts the whole WASM app, though, which wipes this
/// in-memory access token immediately — with nothing else, that alone was
/// enough to bounce an otherwise-still-valid session straight back to /login.
/// GetAuthenticationStateAsync now redeems the persisted refresh token once,
/// on the first check after a cold start, before falling back to anonymous.
/// </summary>
public sealed class JwtAuthenticationStateProvider(ILocalStorageService localStorage, IHttpClientFactory httpClientFactory) : AuthenticationStateProvider
{
    private const string RefreshTokenStorageKey = "retailsphere_refresh_token";

    private static readonly AuthenticationState Anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    private Task? _silentRefreshTask;

    public string? AccessToken { get; private set; }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
        {
            // De-duplicated per app lifetime: concurrent callers (Sidebar, Header,
            // AuthorizeRouteView, ...) all await the same in-flight attempt instead
            // of each firing their own /auth/refresh call.
            _silentRefreshTask ??= TrySilentRefreshAsync();
            await _silentRefreshTask;
        }

        if (string.IsNullOrWhiteSpace(AccessToken))
            return Anonymous;

        var claims = new JwtSecurityTokenHandler().ReadJwtToken(AccessToken).Claims;
        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private async Task TrySilentRefreshAsync()
    {
        string? refreshToken;
        try
        {
            refreshToken = await GetStoredRefreshTokenAsync();
        }
        catch
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(refreshToken))
            return;

        try
        {
            // A plain, unauthenticated client — deliberately not the "RetailSphereApi"
            // client, which carries JwtTokenMessageHandler and injects this very
            // provider, to avoid a circular dependency (and any recursion) at startup.
            var anonymousClient = httpClientFactory.CreateClient("RetailSphereApi.Anonymous");

            var response = await anonymousClient.PostAsJsonAsync(
                "api/v1/auth/refresh", new RefreshTokenRequest { RefreshToken = refreshToken });

            if (!response.IsSuccessStatusCode)
            {
                await LogoutAsync();
                return;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>();
            if (result?.Data is null)
            {
                await LogoutAsync();
                return;
            }

            AccessToken = result.Data.AccessToken;
            await localStorage.SetItemAsStringAsync(RefreshTokenStorageKey, result.Data.RefreshToken);
        }
        catch
        {
            // API unreachable at startup, malformed response, etc. — fail back to
            // anonymous rather than letting an exception escape a framework-invoked
            // authentication check.
            await LogoutAsync();
        }
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
