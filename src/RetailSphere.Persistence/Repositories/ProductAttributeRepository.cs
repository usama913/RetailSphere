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

    /// <summary>
    /// Ignores the soft-delete query filter — see PurchaseOrderRepository.PoNumberExistsAsync
    /// for why: ProductAttributes.HasQueryFilter(a => !a.IsDeleted) would otherwise let
    /// a soft-deleted attribute's name look free forever, while Name's unique index
    /// still rejects the physical duplicate on insert/update.
    /// </summary>
    public Task<bool> NameExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default) =>
        dbContext.ProductAttributes.IgnoreQueryFilters().AnyAsync(a => a.Name == name && (excludeId == null || a.Id != excludeId.Value), cancellationToken);

    public void Add(ProductAttribute attribute) => dbContext.ProductAttributes.Add(attribute);

    public void Update(ProductAttribute attribute) => dbContext.ProductAttributes.Update(attribute);

    public void Remove(ProductAttribute attribute) => dbContext.ProductAttributes.Remove(attribute);
}
