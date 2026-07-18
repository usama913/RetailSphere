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

    public void Add(Brand brand) => dbContext.Brands.Add(brand);

    public void Update(Brand brand) => dbContext.Brands.Update(brand);

    public void Remove(Brand brand) => dbContext.Brands.Remove(brand);
}
