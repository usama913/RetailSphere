using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Persistence.Repositories;

public sealed class ProductRepository(RetailSphereDbContext dbContext) : IProductRepository
{
    public Task<Product?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Products
            .Include(p => p.Variants)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Product> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? search,
        long? categoryId,
        long? brandId,
        bool? isActive,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Products
            .Include(p => p.Variants)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            query = query.Where(p => p.Name.ToLower().Contains(term));
        }

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        if (brandId.HasValue)
            query = query.Where(p => p.BrandId == brandId.Value);

        if (isActive.HasValue)
            query = query.Where(p => p.IsActive == isActive.Value);

        query = query.OrderBy(p => p.Name);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<bool> SkuExistsAsync(string sku, CancellationToken cancellationToken = default) =>
        dbContext.Set<ProductVariant>().AnyAsync(v => v.Sku == sku, cancellationToken);

    public void Add(Product product) => dbContext.Products.Add(product);

    public void Update(Product product) => dbContext.Products.Update(product);

    public void Remove(Product product) => dbContext.Products.Remove(product);
}
