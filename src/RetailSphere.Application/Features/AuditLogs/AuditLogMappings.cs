using RetailSphere.Contracts.Admin;
using RetailSphere.Domain.Auditing;

namespace RetailSphere.Application.Features.AuditLogs;

internal static class AuditLogMappings
{
    public static AuditLogDto ToDto(AuditLog auditLog) => new()
    {
        Id = auditLog.Id,
        TimestampUtc = auditLog.TimestampUtc,
        UserId = auditLog.UserId,
        UserEmail = auditLog.UserEmail,
        EntityType = auditLog.EntityType,
        EntityId = auditLog.EntityId,
        Action = auditLog.Action,
        Details = auditLog.Details,
    };
}
