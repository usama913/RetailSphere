using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Inventory;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<StockItemDto>>> GetStockItemsAsync(
        int page = 1,
        int pageSize = 25,
        long? branchId = null,
        long? productId = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";

        if (productId.HasValue)
            query += $"&productId={productId.Value}";

        return GetAsync<PagedResult<StockItemDto>>($"stock-items?{query}", cancellationToken);
    }

    public Task<ApiResponse<StockItemDto>> GetStockItemByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<StockItemDto>($"stock-items/{id}", cancellationToken);

    public Task<ApiResponse<StockItemDto>> AdjustStockAsync(AdjustStockRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<AdjustStockRequest, StockItemDto>("stock-items/adjust", request, cancellationToken);

    public Task<ApiResponse<object>> TransferStockAsync(TransferStockRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<TransferStockRequest, object>("stock-items/transfer", request, cancellationToken);
}
