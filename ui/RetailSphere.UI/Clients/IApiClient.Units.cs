using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<IReadOnlyList<UnitDto>>> GetUnitsAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<ApiResponse<UnitDto>> CreateUnitAsync(CreateUnitRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<UnitDto>> UpdateUnitAsync(long id, UpdateUnitRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> ActivateUnitAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeactivateUnitAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteUnitAsync(long id, CancellationToken cancellationToken = default);
}
