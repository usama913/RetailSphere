using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<IReadOnlyList<BranchDto>>> GetBranchesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchDto>> CreateBranchAsync(CreateBranchRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<BranchDto>> UpdateBranchAsync(long id, UpdateBranchRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> ActivateBranchAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeactivateBranchAsync(long id, CancellationToken cancellationToken = default);
}
