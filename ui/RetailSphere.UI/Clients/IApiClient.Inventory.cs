using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Inventory;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<StockItemDto>>> GetStockItemsAsync(
        int page = 1,
        int pageSize = 25,
        long? branchId = null,
        long? productId = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<StockItemDto>> GetStockItemByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<StockItemDto>> AdjustStockAsync(AdjustStockRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> TransferStockAsync(TransferStockRequest request, CancellationToken cancellationToken = default);
}
