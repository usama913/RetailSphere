using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<IReadOnlyList<BranchDto>>> GetBranchesAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<BranchDto>>($"branches?includeInactive={includeInactive}", cancellationToken);

    public Task<ApiResponse<BranchDto>> CreateBranchAsync(CreateBranchRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateBranchRequest, BranchDto>("branches", request, cancellationToken);

    public Task<ApiResponse<BranchDto>> UpdateBranchAsync(long id, UpdateBranchRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateBranchRequest, BranchDto>($"branches/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> ActivateBranchAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"branches/{id}/activate", cancellationToken);

    public Task<ApiResponse<object>> DeactivateBranchAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"branches/{id}/deactivate", cancellationToken);
}
