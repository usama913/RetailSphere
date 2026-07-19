using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<SupplierPaymentDto>>> GetSupplierPaymentsAsync(
        int page = 1,
        int pageSize = 25,
        long? supplierId = null,
        long? purchaseInvoiceId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (supplierId.HasValue)
            query += $"&supplierId={supplierId.Value}";

        if (purchaseInvoiceId.HasValue)
            query += $"&purchaseInvoiceId={purchaseInvoiceId.Value}";

        if (fromDate.HasValue)
            query += $"&fromDate={fromDate.Value:yyyy-MM-dd}";

        if (toDate.HasValue)
            query += $"&toDate={toDate.Value:yyyy-MM-dd}";

        return GetAsync<PagedResult<SupplierPaymentDto>>($"supplier-payments?{query}", cancellationToken);
    }

    public Task<ApiResponse<SupplierPaymentDto>> GetSupplierPaymentByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<SupplierPaymentDto>($"supplier-payments/{id}", cancellationToken);

    public Task<ApiResponse<SupplierPaymentDto>> RecordSupplierPaymentAsync(RecordSupplierPaymentRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<RecordSupplierPaymentRequest, SupplierPaymentDto>("supplier-payments", request, cancellationToken);

    public Task<ApiResponse<SupplierPaymentDto>> UpdateSupplierPaymentAsync(long id, UpdateSupplierPaymentRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateSupplierPaymentRequest, SupplierPaymentDto>($"supplier-payments/{id}", request, cancellationToken);

    public Task<ApiResponse<SupplierPaymentDto>> ReverseSupplierPaymentAsync(long id, ReverseSupplierPaymentRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<ReverseSupplierPaymentRequest, SupplierPaymentDto>($"supplier-payments/{id}/reverse", request, cancellationToken);
}
