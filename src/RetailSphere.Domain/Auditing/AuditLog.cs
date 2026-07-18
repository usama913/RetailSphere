using RetailSphere.SharedKernel;

namespace RetailSphere.Domain.Auditing;

/// <summary>
/// An immutable record of an administrative action (Phase 1 — Admin &amp; Identity).
/// Written explicitly by the application-layer command handlers that perform
/// user-visible admin actions (create/update/deactivate a User, Role, or Branch,
/// grant/revoke a role or permission, reset a password, ...) rather than captured
/// generically off the EF change tracker — this keeps every entry meaningful and
/// human-readable instead of a raw column diff.
/// </summary>
public sealed class AuditLog : Entity<long>
{
    public DateTime TimestampUtc { get; private set; }

    /// <summary>Null for system-initiated actions (e.g. the development seeder).</summary>
    public long? UserId { get; private set; }

    /// <summary>Denormalized at write time so the trail stays readable even if the acting user is later deleted.</summary>
    public string? UserEmail { get; private set; }

    public string EntityType { get; private set; } = default!;

    public string EntityId { get; private set; } = default!;

    public string Action { get; private set; } = default!;

    /// <summary>Short human-readable summary of what changed (e.g. "Granted role 'Branch Manager'").</summary>
    public string? Details { get; private set; }

    private AuditLog()
    {
    }

    public static AuditLog Create(
        long? userId,
        string? userEmail,
        string entityType,
        string entityId,
        string action,
        string? details = null) => new()
    {
        TimestampUtc = DateTime.UtcNow,
        UserId = userId,
        UserEmail = userEmail,
        EntityType = entityType,
        EntityId = entityId,
        Action = action,
        Details = details,
    };
}
