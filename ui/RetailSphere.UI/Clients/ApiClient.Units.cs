using RetailSphere.Contracts.Catalog;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<IReadOnlyList<UnitDto>>> GetUnitsAsync(bool includeInactive = false, CancellationToken cancellationToken = default) =>
        GetAsync<IReadOnlyList<UnitDto>>($"units?includeInactive={includeInactive}", cancellationToken);

    public Task<ApiResponse<UnitDto>> CreateUnitAsync(CreateUnitRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateUnitRequest, UnitDto>("units", request, cancellationToken);

    public Task<ApiResponse<UnitDto>> UpdateUnitAsync(long id, UpdateUnitRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateUnitRequest, UnitDto>($"units/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> ActivateUnitAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"units/{id}/activate", cancellationToken);

    public Task<ApiResponse<object>> DeactivateUnitAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<object>($"units/{id}/deactivate", cancellationToken);

    public Task<ApiResponse<object>> DeleteUnitAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"units/{id}", cancellationToken);
}
