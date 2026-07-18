namespace RetailSphere.Domain.Purchasing;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Supplier>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    void Add(Supplier supplier);

    void Update(Supplier supplier);

    void Remove(Supplier supplier);
}
