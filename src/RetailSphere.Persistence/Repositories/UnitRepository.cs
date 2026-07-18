using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Catalog;

namespace RetailSphere.Persistence.Repositories;

public sealed class UnitRepository(RetailSphereDbContext dbContext) : IUnitRepository
{
    public Task<Unit?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Units.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Unit>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Units.AsQueryable();
        if (!includeInactive) query = query.Where(u => u.IsActive);
        return await query.OrderBy(u => u.Name).ToListAsync(cancellationToken);
    }

    public void Add(Unit unit) => dbContext.Units.Add(unit);

    public void Update(Unit unit) => dbContext.Units.Update(unit);

    public void Remove(Unit unit) => dbContext.Units.Remove(unit);
}
