using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<PurchaseOrderDto>>> GetPurchaseOrdersAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? supplierId = null,
        long? branchId = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        if (supplierId.HasValue)
            query += $"&supplierId={supplierId.Value}";

        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";

        if (!string.IsNullOrWhiteSpace(status))
            query += $"&status={Uri.EscapeDataString(status)}";

        return GetAsync<PagedResult<PurchaseOrderDto>>($"purchase-orders?{query}", cancellationToken);
    }

    public Task<ApiResponse<PurchaseOrderDto>> GetPurchaseOrderByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<PurchaseOrderDto>($"purchase-orders/{id}", cancellationToken);

    public Task<ApiResponse<PurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreatePurchaseOrderRequest, PurchaseOrderDto>("purchase-orders", request, cancellationToken);

    public Task<ApiResponse<PurchaseOrderDto>> UpdatePurchaseOrderAsync(long id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdatePurchaseOrderRequest, PurchaseOrderDto>($"purchase-orders/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> DeletePurchaseOrderAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"purchase-orders/{id}", cancellationToken);

    public Task<ApiResponse<PurchaseOrderDto>> AddPurchaseOrderLineAsync(long purchaseOrderId, AddPurchaseOrderLineRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<AddPurchaseOrderLineRequest, PurchaseOrderDto>($"purchase-orders/{purchaseOrderId}/lines", request, cancellationToken);

    public Task<ApiResponse<PurchaseOrderDto>> UpdatePurchaseOrderLineAsync(long purchaseOrderId, long lineId, UpdatePurchaseOrderLineRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdatePurchaseOrderLineRequest, PurchaseOrderDto>($"purchase-orders/{purchaseOrderId}/lines/{lineId}", request, cancellationToken);

    public Task<ApiResponse<PurchaseOrderDto>> RemovePurchaseOrderLineAsync(long purchaseOrderId, long lineId, CancellationToken cancellationToken = default) =>
        DeleteAsync<PurchaseOrderDto>($"purchase-orders/{purchaseOrderId}/lines/{lineId}", cancellationToken);

    public Task<ApiResponse<PurchaseOrderDto>> SubmitPurchaseOrderAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<PurchaseOrderDto>($"purchase-orders/{id}/submit", cancellationToken);

    public Task<ApiResponse<PurchaseOrderDto>> ReceivePurchaseOrderLineAsync(long purchaseOrderId, long lineId, ReceivePurchaseOrderLineRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<ReceivePurchaseOrderLineRequest, PurchaseOrderDto>($"purchase-orders/{purchaseOrderId}/lines/{lineId}/receive", request, cancellationToken);

    public Task<ApiResponse<PurchaseOrderDto>> CancelPurchaseOrderAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<PurchaseOrderDto>($"purchase-orders/{id}/cancel", cancellationToken);
}
