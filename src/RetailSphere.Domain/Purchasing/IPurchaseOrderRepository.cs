namespace RetailSphere.Domain.Purchasing;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<PurchaseOrder> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? search,
        long? supplierId,
        long? branchId,
        string? status,
        CancellationToken cancellationToken = default);

    Task<bool> PoNumberExistsAsync(string poNumber, CancellationToken cancellationToken = default);

    void Add(PurchaseOrder purchaseOrder);

    void Update(PurchaseOrder purchaseOrder);

    void Remove(PurchaseOrder purchaseOrder);
}
