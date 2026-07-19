using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<PurchaseInvoiceDto>>> GetPurchaseInvoicesAsync(
        int page = 1,
        int pageSize = 25,
        long? supplierId = null,
        long? branchId = null,
        string? paymentStatus = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (supplierId.HasValue)
            query += $"&supplierId={supplierId.Value}";

        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";

        if (!string.IsNullOrWhiteSpace(paymentStatus))
            query += $"&paymentStatus={Uri.EscapeDataString(paymentStatus)}";

        if (fromDate.HasValue)
            query += $"&fromDate={fromDate.Value:yyyy-MM-dd}";

        if (toDate.HasValue)
            query += $"&toDate={toDate.Value:yyyy-MM-dd}";

        return GetAsync<PagedResult<PurchaseInvoiceDto>>($"purchase-invoices?{query}", cancellationToken);
    }

    public Task<ApiResponse<PurchaseInvoiceDto>> GetPurchaseInvoiceByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<PurchaseInvoiceDto>($"purchase-invoices/{id}", cancellationToken);

    public Task<ApiResponse<PurchaseInvoiceDto>> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreatePurchaseInvoiceRequest, PurchaseInvoiceDto>("purchase-invoices", request, cancellationToken);

    public Task<ApiResponse<PurchaseInvoiceDto>> UpdatePurchaseInvoiceAsync(long id, UpdatePurchaseInvoiceRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdatePurchaseInvoiceRequest, PurchaseInvoiceDto>($"purchase-invoices/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> DeletePurchaseInvoiceAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"purchase-invoices/{id}", cancellationToken);
}
