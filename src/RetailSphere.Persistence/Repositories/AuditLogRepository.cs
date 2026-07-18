using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Auditing;

namespace RetailSphere.Persistence.Repositories;

public sealed class AuditLogRepository(RetailSphereDbContext dbContext) : IAuditLogRepository
{
    public void Add(AuditLog auditLog) => dbContext.AuditLogs.Add(auditLog);

    public async Task<(IReadOnlyList<AuditLog> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? entityType,
        string? action,
        long? userId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(a => a.EntityType == entityType);

        if (!string.IsNullOrWhiteSpace(action))
            query = query.Where(a => a.Action == action);

        if (userId.HasValue)
            query = query.Where(a => a.UserId == userId.Value);

        if (fromUtc.HasValue)
            query = query.Where(a => a.TimestampUtc >= fromUtc.Value);

        if (toUtc.HasValue)
            query = query.Where(a => a.TimestampUtc <= toUtc.Value);

        query = query.OrderByDescending(a => a.TimestampUtc);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
