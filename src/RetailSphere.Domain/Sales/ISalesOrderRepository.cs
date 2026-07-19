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

    /// <summary>Every order for one customer, oldest first — feeds the Customer Ledger.</summary>
    Task<IReadOnlyList<SalesOrder>> GetByCustomerAsync(long customerId, CancellationToken cancellationToken = default);

    /// <summary>Non-cancelled orders for one customer with a non-zero OutstandingBalance — feeds the Customer Payment screen's "outstanding invoices" picker and the POS credit summary panel.</summary>
    Task<IReadOnlyList<SalesOrder>> GetOutstandingByCustomerAsync(long customerId, CancellationToken cancellationToken = default);

    /// <summary>Every non-cancelled order across all customers with a non-zero OutstandingBalance — feeds the Customer Aging report and the "Total Customer Outstanding" dashboard widget.</summary>
    Task<IReadOnlyList<SalesOrder>> GetOutstandingAsync(CancellationToken cancellationToken = default);

    /// <summary>Ignores the soft-delete filter — same reasoning as PurchaseOrderRepository.PoNumberExistsAsync.</summary>
    Task<bool> OrderNumberExistsAsync(string orderNumber, CancellationToken cancellationToken = default);

    void Add(SalesOrder salesOrder);

    void Update(SalesOrder salesOrder);

    void Remove(SalesOrder salesOrder);
}
