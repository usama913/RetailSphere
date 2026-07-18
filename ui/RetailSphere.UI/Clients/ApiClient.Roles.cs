using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<IReadOnlyList<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<RoleDto>>("roles", cancellationToken);

    public Task<ApiResponse<IReadOnlyList<PermissionDto>>> GetPermissionsAsync(CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<PermissionDto>>("roles/permissions", cancellationToken);

    public Task<ApiResponse<RoleDto>> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateRoleRequest, RoleDto>("roles", request, cancellationToken);

    public Task<ApiResponse<RoleDto>> UpdateRoleAsync(long id, UpdateRoleRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateRoleRequest, RoleDto>($"roles/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> DeleteRoleAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"roles/{id}", cancellationToken);
}
