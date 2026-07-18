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

    /// <summary>
    /// Ignores the soft-delete query filter — see PurchaseOrderRepository.PoNumberExistsAsync
    /// for why: Units.HasQueryFilter(u => !u.IsDeleted) would otherwise let a
    /// soft-deleted unit's name look free forever, while Name's unique index still
    /// rejects the physical duplicate on insert/update.
    /// </summary>
    public Task<bool> NameExistsAsync(string name, long? excludeId = null, CancellationToken cancellationToken = default) =>
        dbContext.Units.IgnoreQueryFilters().AnyAsync(u => u.Name == name && (excludeId == null || u.Id != excludeId.Value), cancellationToken);

    public void Add(Unit unit) => dbContext.Units.Add(unit);

    public void Update(Unit unit) => dbContext.Units.Update(unit);

    public void Remove(Unit unit) => dbContext.Units.Remove(unit);
}
