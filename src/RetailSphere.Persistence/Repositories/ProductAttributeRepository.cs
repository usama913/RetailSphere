using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Persistence.Repositories;

public sealed class ProductAttributeRepository(RetailSphereDbContext dbContext) : IProductAttributeRepository
{
    public Task<ProductAttribute?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.ProductAttributes
            .Include(a => a.Values)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProductAttribute>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await dbContext.ProductAttributes
            .Include(a => a.Values)
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

    public void Add(ProductAttribute attribute) => dbContext.ProductAttributes.Add(attribute);

    public void Update(ProductAttribute attribute) => dbContext.ProductAttributes.Update(attribute);

    public void Remove(ProductAttribute attribute) => dbContext.ProductAttributes.Remove(attribute);
}
