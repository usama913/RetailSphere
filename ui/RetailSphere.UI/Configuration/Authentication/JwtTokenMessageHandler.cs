using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Http;
using RetailSphere.Contracts.Auth;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Configuration.Authentication;

/// <summary>
/// Attaches the current access token to every outgoing request and, on a 401,
/// attempts exactly one silent refresh (via a bare HttpClient, not the one this
/// handler is attached to, to avoid recursing through itself) before retrying.
/// If refresh also fails, the user is signed out and the caller sees the 401.
/// </summary>
public sealed class JwtTokenMessageHandler(JwtAuthenticationStateProvider authStateProvider, IHttpClientFactory httpClientFactory)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(authStateProvider.AccessToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authStateProvider.AccessToken);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
            return response;

        var refreshed = await TryRefreshAsync(cancellationToken);
        if (!refreshed)
            return response;

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authStateProvider.AccessToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<bool> TryRefreshAsync(CancellationToken cancellationToken)
    {
        var refreshToken = await authStateProvider.GetStoredRefreshTokenAsync();
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        // A plain client with no auth handler attached — avoids infinite recursion.
        var anonymousClient = httpClientFactory.CreateClient("RetailSphereApi.Anonymous");

        var response = await anonymousClient.PostAsJsonAsync(
            "api/v1/auth/refresh", new RefreshTokenRequest { RefreshToken = refreshToken }, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            await authStateProvider.LogoutAsync();
            return false;
        }

        var result = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(cancellationToken);
        if (result?.Data is null)
        {
            await authStateProvider.LogoutAsync();
            return false;
        }

        await authStateProvider.SetSessionAsync(result.Data.AccessToken, result.Data.RefreshToken);
        return true;
    }
}
