using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Purchasing;

namespace RetailSphere.Persistence.Repositories;

public sealed class SupplierRepository(RetailSphereDbContext dbContext) : ISupplierRepository
{
    public Task<Supplier?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Suppliers.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Supplier>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Suppliers.AsQueryable();
        if (!includeInactive) query = query.Where(s => s.IsActive);
        return await query.OrderBy(s => s.Name).ToListAsync(cancellationToken);
    }

    public void Add(Supplier supplier) => dbContext.Suppliers.Add(supplier);

    public void Update(Supplier supplier) => dbContext.Suppliers.Update(supplier);

    public void Remove(Supplier supplier) => dbContext.Suppliers.Remove(supplier);
}
