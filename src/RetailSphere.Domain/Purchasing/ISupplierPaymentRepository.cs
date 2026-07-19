namespace RetailSphere.Domain.Purchasing;

public interface ISupplierPaymentRepository
{
    Task<SupplierPayment?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<SupplierPayment> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? supplierId,
        long? purchaseInvoiceId,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    /// <summary>Every non-reversed and reversed payment for one supplier, oldest first — feeds the Supplier Ledger.</summary>
    Task<IReadOnlyList<SupplierPayment>> GetBySupplierAsync(long supplierId, CancellationToken cancellationToken = default);

    void Add(SupplierPayment payment);

    void Update(SupplierPayment payment);
}
