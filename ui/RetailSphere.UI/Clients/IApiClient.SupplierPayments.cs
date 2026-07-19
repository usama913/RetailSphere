using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<SupplierPaymentDto>>> GetSupplierPaymentsAsync(
        int page = 1,
        int pageSize = 25,
        long? supplierId = null,
        long? purchaseInvoiceId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<SupplierPaymentDto>> GetSupplierPaymentByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<SupplierPaymentDto>> RecordSupplierPaymentAsync(RecordSupplierPaymentRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<SupplierPaymentDto>> UpdateSupplierPaymentAsync(long id, UpdateSupplierPaymentRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<SupplierPaymentDto>> ReverseSupplierPaymentAsync(long id, ReverseSupplierPaymentRequest request, CancellationToken cancellationToken = default);
}
