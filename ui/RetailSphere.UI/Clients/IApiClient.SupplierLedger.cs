using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<SupplierLedgerDto>> GetSupplierLedgerAsync(long supplierId, CancellationToken cancellationToken = default);

    Task<ApiResponse<SupplierAgingReportDto>> GetSupplierAgingReportAsync(long? supplierId = null, CancellationToken cancellationToken = default);
}
