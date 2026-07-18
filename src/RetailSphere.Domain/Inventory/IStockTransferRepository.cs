namespace RetailSphere.Domain.Inventory;

public interface IStockTransferRepository
{
    Task<StockTransfer?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<StockTransfer> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? search,
        long? fromBranchId,
        long? toBranchId,
        string? status,
        CancellationToken cancellationToken = default);

    /// <summary>Ignores the soft-delete filter — see StockTransferRepository.TransferNumberExistsAsync.</summary>
    Task<bool> TransferNumberExistsAsync(string transferNumber, CancellationToken cancellationToken = default);

    void Add(StockTransfer stockTransfer);

    void Update(StockTransfer stockTransfer);

    void Remove(StockTransfer stockTransfer);
}
