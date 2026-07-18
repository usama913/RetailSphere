using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<PurchaseOrderDto>>> GetPurchaseOrdersAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? supplierId = null,
        long? branchId = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseOrderDto>> GetPurchaseOrderByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseOrderDto>> CreatePurchaseOrderAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseOrderDto>> UpdatePurchaseOrderAsync(long id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeletePurchaseOrderAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseOrderDto>> AddPurchaseOrderLineAsync(long purchaseOrderId, AddPurchaseOrderLineRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseOrderDto>> UpdatePurchaseOrderLineAsync(long purchaseOrderId, long lineId, UpdatePurchaseOrderLineRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseOrderDto>> RemovePurchaseOrderLineAsync(long purchaseOrderId, long lineId, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseOrderDto>> SubmitPurchaseOrderAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseOrderDto>> ReceivePurchaseOrderLineAsync(long purchaseOrderId, long lineId, ReceivePurchaseOrderLineRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<PurchaseOrderDto>> CancelPurchaseOrderAsync(long id, CancellationToken cancellationToken = default);
}
