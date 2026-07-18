using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Persistence.Repositories;

public sealed class CategoryRepository(RetailSphereDbContext dbContext) : ICategoryRepository
{
    public Task<Category?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Category>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Categories.AsQueryable();
        if (!includeInactive) query = query.Where(c => c.IsActive);
        return await query.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public Task<bool> HasChildrenAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Categories.AnyAsync(c => c.ParentCategoryId == id, cancellationToken);

    public void Add(Category category) => dbContext.Categories.Add(category);

    public void Update(Category category) => dbContext.Categories.Update(category);

    public void Remove(Category category) => dbContext.Categories.Remove(category);
}
