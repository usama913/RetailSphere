namespace RetailSphere.Domain.Sales;

public interface ISalesOrderRepository
{
    Task<SalesOrder?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<SalesOrder> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? search,
        long? branchId,
        long? customerId,
        string? status,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    /// <summary>Ignores the soft-delete filter — same reasoning as PurchaseOrderRepository.PoNumberExistsAsync.</summary>
    Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default);

    void Add(SalesOrder salesOrder);

    void Update(SalesOrder salesOrder);

    void Remove(SalesOrder salesOrder);
}
