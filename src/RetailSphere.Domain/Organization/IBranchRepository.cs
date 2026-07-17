namespace RetailSphere.Domain.Organization;

public interface IBranchRepository
{
    Task<Branch?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Branch>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);

    Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default);

    void Add(Branch branch);

    void Update(Branch branch);
}
