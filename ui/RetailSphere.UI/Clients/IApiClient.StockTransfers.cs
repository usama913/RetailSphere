using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Inventory;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<StockTransferDto>>> GetStockTransfersAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? fromBranchId = null,
        long? toBranchId = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<StockTransferDto>> GetStockTransferByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<StockTransferDto>> CreateStockTransferAsync(CreateStockTransferRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<StockTransferDto>> UpdateStockTransferAsync(long id, UpdateStockTransferRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<object>> DeleteStockTransferAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<StockTransferDto>> AddStockTransferLineAsync(long stockTransferId, AddStockTransferLineRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<StockTransferDto>> UpdateStockTransferLineAsync(long stockTransferId, long lineId, UpdateStockTransferLineRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<StockTransferDto>> RemoveStockTransferLineAsync(long stockTransferId, long lineId, CancellationToken cancellationToken = default);

    Task<ApiResponse<StockTransferDto>> SubmitStockTransferAsync(long id, CancellationToken cancellationToken = default);

    Task<ApiResponse<StockTransferDto>> ReceiveStockTransferLineAsync(long stockTransferId, long lineId, ReceiveStockTransferLineRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<StockTransferDto>> CancelStockTransferAsync(long id, CancellationToken cancellationToken = default);
}
