namespace RetailSphere.Domain.Auditing;

public interface IAuditLogRepository
{
    void Add(AuditLog auditLog);

    Task<(IReadOnlyList<AuditLog> Items, long TotalCount)> SearchAsync(
        int page,
        int pageSize,
        string? entityType,
        string? action,
        long? userId,
        DateTime? fromUtc,
        DateTime? toUtc,
        CancellationToken cancellationToken = default);
}
