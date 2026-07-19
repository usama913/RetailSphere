namespace RetailSphere.Domain.Customers;

public interface ICustomerPaymentRepository
{
    Task<CustomerPayment?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<CustomerPayment> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? customerId,
        long? branchId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    /// <summary>Every non-reversed and reversed payment for one customer, oldest first — feeds the Customer Ledger.</summary>
    Task<IReadOnlyList<CustomerPayment>> GetByCustomerAsync(long customerId, CancellationToken cancellationToken = default);

    void Add(CustomerPayment payment);

    void Update(CustomerPayment payment);
}
