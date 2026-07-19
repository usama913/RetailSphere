using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Finance;

namespace RetailSphere.Persistence.Repositories;

public sealed class CashRegisterSessionRepository(RetailSphereDbContext dbContext) : ICashRegisterSessionRepository
{
    public Task<CashRegisterSession?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.CashRegisterSessions.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<CashRegisterSession?> GetOpenSessionAsync(long branchId, CancellationToken cancellationToken = default) =>
        dbContext.CashRegisterSessions.FirstOrDefaultAsync(s => s.BranchId == branchId && s.Status == "Open", cancellationToken);

    public async Task<(IReadOnlyList<CashRegisterSession> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        long? branchId,
        string? status,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.CashRegisterSessions.AsQueryable();

        if (branchId.HasValue)
            query = query.Where(s => s.BranchId == branchId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.Status == status);

        query = query.OrderByDescending(s => s.OpenedAtUtc);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public void Add(CashRegisterSession session) => dbContext.CashRegisterSessions.Add(session);

    public void Update(CashRegisterSession session) => dbContext.CashRegisterSessions.Update(session);
}
