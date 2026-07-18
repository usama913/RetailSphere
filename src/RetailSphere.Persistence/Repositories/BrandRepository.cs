using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Persistence.Repositories;

public sealed class BrandRepository(RetailSphereDbContext dbContext) : IBrandRepository
{
    public Task<Brand?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Brands.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Brand>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Brands.AsQueryable();
        if (!includeInactive) query = query.Where(b => b.IsActive);
        return await query.OrderBy(b => b.Name).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Ignores the soft-delete query filter — see PurchaseOrderRepository.PoNumberExistsAsync
    /// for why: Brands.HasQueryFilter(b => !b.IsDeleted) would otherwise let a
    /// soft-deleted brand's name look free forever, while Name's unique index still
    /// rejects the physical duplicate on insert/update.
    /// </summary>
    public Task<bool> NameExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default) =>
        dbContext.Brands.IgnoreQueryFilters().AnyAsync(b => b.Name == name && (excludeId == null || b.Id != excludeId.Value), cancellationToken);

    public void Add(Brand brand) => dbContext.Brands.Add(brand);

    public void Update(Brand brand) => dbContext.Brands.Update(brand);

    public void Remove(Brand brand) => dbContext.Brands.Remove(brand);
}
