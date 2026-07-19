namespace RetailSphere.Domain.Purchasing;

public interface IPurchaseInvoiceRepository
{
    Task<PurchaseInvoice?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<PurchaseInvoice> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? supplierId,
        long? branchId,
        string? paymentStatus,
        DateTime? fromDate,
        DateTime? toDate,
        CancellationToken cancellationToken = default);

    /// <summary>Every invoice for one supplier, oldest first — the backbone of the Supplier Ledger (see SupplierLedgerQueryHandler).</summary>
    Task<IReadOnlyList<PurchaseInvoice>> GetBySupplierAsync(long supplierId, CancellationToken cancellationToken = default);

    /// <summary>Every invoice with a non-zero outstanding balance, across all suppliers — feeds the Aging Report and the "Total Supplier Outstanding" dashboard widget.</summary>
    Task<IReadOnlyList<PurchaseInvoice>> GetOutstandingAsync(long? supplierId = null, CancellationToken cancellationToken = default);

    Task<bool> SupplierInvoiceNumberExistsAsync(long supplierId, string supplierInvoiceNumber, long? excludeId = null, CancellationToken cancellationToken = default);

    void Add(PurchaseInvoice invoice);

    void Update(PurchaseInvoice invoice);

    void Remove(PurchaseInvoice invoice);
}
