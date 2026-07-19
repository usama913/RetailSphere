namespace RetailSphere.Domain.Sales;

public interface ISalesReturnRepository
{
    Task<SalesReturn?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<SalesReturn> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? branchId,
        long? customerId,
        long? salesOrderId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    /// <summary>Ignores the soft-delete filter — same reasoning as SalesOrderRepository.OrderNumberExistsAsync.</summary>
    Task<bool> ReturnNumberExistsAsync(string returnNumber, CancellationToken cancellationToken = default);

    void Add(SalesReturn salesReturn);

    void Remove(SalesReturn salesReturn);
}
