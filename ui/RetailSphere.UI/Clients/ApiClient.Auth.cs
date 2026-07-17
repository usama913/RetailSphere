using RetailSphere.Contracts.Auth;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<LoginRequest, LoginResponse>("auth/login", request, cancellationToken);

    public Task<ApiResponse<LoginResponse>> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<RefreshTokenRequest, LoginResponse>("auth/refresh", request, cancellationToken);

    public Task<ApiResponse<object>> LogoutAsync(LogoutRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<LogoutRequest, object>("auth/logout", request, cancellationToken);
}
