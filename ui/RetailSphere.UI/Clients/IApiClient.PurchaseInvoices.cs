using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<PurchaseInvoiceDto>>> GetPurchaseInvoicesAsync(
        int page = 1,
        int pageSize = 25,
        long? supplierId = null,
        long? branchId = null,
        string? paymentStatus = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseInvoiceDto>> GetPurchaseInvoiceByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseInvoiceDto>> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseInvoiceDto>> UpdatePurchaseInvoiceAsync(long id, UpdatePurchaseInvoiceRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeletePurchaseInvoiceAsync(long id, CancellationToken cancellationToken = default);
}
