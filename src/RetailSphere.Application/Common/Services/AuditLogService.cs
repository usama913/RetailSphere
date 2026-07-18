using RetailSphere.Common;
using RetailSphere.Domain.Auditing;

namespace RetailSphere.Application.Common.Services;

/// <summary>
/// Thin helper so every admin command handler writes audit trail entries the
/// same way (Phase 1 — Admin &amp; Identity). Deliberately explicit — called once,
/// right after the domain operation succeeds, rather than captured generically
/// off the EF change tracker, so every entry is a meaningful, readable sentence
/// instead of a raw column diff. Staged via the repository like any other
/// change; the calling handler still owns the single IUnitOfWork.SaveChangesAsync
/// call that commits it.
/// </summary>
public sealed class AuditLogService(IAuditLogRepository auditLogRepository, ICurrentUserService currentUserService)
{
    public void Log(string entityType, string entityId, string action, string? details = null)
    {
        var entry = AuditLog.Create(
            currentUserService.UserId,
            currentUserService.Email,
            entityType,
            entityId,
            action,
            details);

        auditLogRepository.Add(entry);
    }
}
