using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<UserDto>>> GetUsersAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? branchId = null,
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<UserDto>> GetUserByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<UserDto>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<UserDto>> UpdateUserAsync(long id, UpdateUserRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<UserDto>> AssignUserRolesAsync(long id, AssignRolesRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> ActivateUserAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeactivateUserAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> ResetUserPasswordAsync(long id, ResetPasswordRequest request, CancellationToken cancellationToken = default);
}
