using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Organization;

namespace RetailSphere.Persistence.Repositories;

public sealed class BranchRepository(RetailSphereDbContext dbContext) : IBranchRepository
{
    public Task<Branch?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Branches.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Branch>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Branches.AsQueryable();
        if (!includeInactive) query = query.Where(b => b.IsActive);
        return await query.ToListAsync(cancellationToken);
    }

    public Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default) =>
        dbContext.Branches.AnyAsync(b => b.Code == code.ToUpper(), cancellationToken);

    public void Add(Branch branch) => dbContext.Branches.Add(branch);

    public void Update(Branch branch) => dbContext.Branches.Update(branch);
}
