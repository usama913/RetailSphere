using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<SupplierLedgerDto>> GetSupplierLedgerAsync(long supplierId, CancellationToken cancellationToken = default) =>
        GetAsync<SupplierLedgerDto>($"suppliers/{supplierId}/ledger", cancellationToken);

    public Task<ApiResponse<SupplierAgingReportDto>> GetSupplierAgingReportAsync(long? supplierId = null, CancellationToken cancellationToken = default)
    {
        var query = supplierId.HasValue ? $"?supplierId={supplierId.Value}" : string.Empty;
        return GetAsync<SupplierAgingReportDto>($"suppliers/aging-report{query}", cancellationToken);
    }
}
