namespace RetailSphere.Domain.Catalog;

public interface IUnitRepository
{
    Task<Unit?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Unit>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    void Add(Unit unit);

    void Update(Unit unit);

    void Remove(Unit unit);
}
