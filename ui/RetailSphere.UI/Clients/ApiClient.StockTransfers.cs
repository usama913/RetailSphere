using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Inventory;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<StockTransferDto>>> GetStockTransfersAsync(
        int page = 1,
        int pageSize = 25,
        string? search = null,
        long? fromBranchId = null,
        long? toBranchId = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={Uri.EscapeDataString(search)}";

        if (fromBranchId.HasValue)
            query += $"&fromBranchId={fromBranchId.Value}";

        if (toBranchId.HasValue)
            query += $"&toBranchId={toBranchId.Value}";

        if (!string.IsNullOrWhiteSpace(status))
            query += $"&status={Uri.EscapeDataString(status)}";

        return GetAsync<PagedResult<StockTransferDto>>($"stock-transfers?{query}", cancellationToken);
    }

    public Task<ApiResponse<StockTransferDto>> GetStockTransferByIdAsync(long id, CancellationToken cancellationToken = default) =>
        GetAsync<StockTransferDto>($"stock-transfers/{id}", cancellationToken);

    public Task<ApiResponse<StockTransferDto>> CreateStockTransferAsync(CreateStockTransferRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CreateStockTransferRequest, StockTransferDto>("stock-transfers", request, cancellationToken);

    public Task<ApiResponse<StockTransferDto>> UpdateStockTransferAsync(long id, UpdateStockTransferRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateStockTransferRequest, StockTransferDto>($"stock-transfers/{id}", request, cancellationToken);

    public Task<ApiResponse<object>> DeleteStockTransferAsync(long id, CancellationToken cancellationToken = default) =>
        DeleteAsync<object>($"stock-transfers/{id}", cancellationToken);

    public Task<ApiResponse<StockTransferDto>> AddStockTransferLineAsync(long stockTransferId, AddStockTransferLineRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<AddStockTransferLineRequest, StockTransferDto>($"stock-transfers/{stockTransferId}/lines", request, cancellationToken);

    public Task<ApiResponse<StockTransferDto>> UpdateStockTransferLineAsync(long stockTransferId, long lineId, UpdateStockTransferLineRequest request, CancellationToken cancellationToken = default) =>
        PutAsync<UpdateStockTransferLineRequest, StockTransferDto>($"stock-transfers/{stockTransferId}/lines/{lineId}", request, cancellationToken);

    public Task<ApiResponse<StockTransferDto>> RemoveStockTransferLineAsync(long stockTransferId, long lineId, CancellationToken cancellationToken = default) =>
        DeleteAsync<StockTransferDto>($"stock-transfers/{stockTransferId}/lines/{lineId}", cancellationToken);

    public Task<ApiResponse<StockTransferDto>> SubmitStockTransferAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<StockTransferDto>($"stock-transfers/{id}/submit", cancellationToken);

    public Task<ApiResponse<StockTransferDto>> ReceiveStockTransferLineAsync(long stockTransferId, long lineId, ReceiveStockTransferLineRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<ReceiveStockTransferLineRequest, StockTransferDto>($"stock-transfers/{stockTransferId}/lines/{lineId}/receive", request, cancellationToken);

    public Task<ApiResponse<StockTransferDto>> CancelStockTransferAsync(long id, CancellationToken cancellationToken = default) =>
        PostAsync<StockTransferDto>($"stock-transfers/{id}/cancel", cancellationToken);
}
