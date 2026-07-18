using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? branchId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";

        if (isActive.HasValue)
            query += $"&isActive={isActive.Value}";

        return GetAsync<PagedResult<UserDto>>($"users?{query}", cancellationToken);
    }

    public Task<ApiResponse<UserDto>> GetUserByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<UserDto>($"users/{id}", cancellationToken);

    public Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateUserRequest, UserDto>("users", request, cancellationToken);

    public Task<ApiResponse<UserDto>> UpdateUserAsync(long id, UpdateUserRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateUserRequest, UserDto>($"users/{id}", request, cancellationToken);

    public Task<ApiResponse<UserDto>> AssignUserRolesAsync(long id, AssignRolesRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<AssignRolesRequest, UserDto>($"users/{id}/roles", request, cancellationToken);

    public Task<ApiResponse<object>> ActivateUserAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"users/{id}/activate", cancellationToken);

    public Task<ApiResponse<object>> DeactivateUserAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"users/{id}/deactivate", cancellationToken);

    public Task<ApiResponse<object>> ResetUserPasswordAsync(long id, ResetPasswordRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<ResetPasswordRequest, object>($"users/{id}/reset-password", request, cancellationToken);
}
