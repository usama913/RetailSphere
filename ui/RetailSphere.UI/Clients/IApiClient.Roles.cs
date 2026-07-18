using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<IReadOnlyList<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<IReadOnlyList<PermissionDto>>> GetPermissionsAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<RoleDto>> UpdateRoleAsync(long id, UpdateRoleRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteRoleAsync(long id, CancellationToken cancellationToken = default);
}
