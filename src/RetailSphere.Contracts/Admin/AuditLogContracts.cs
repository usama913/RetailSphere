namespace RetailSphere.Contracts.Admin;

public sealed class AuditLogDto
{
    public required long Id { get; init; }

    public required DateTime TimestampUtc { get; init; }

    public long? UserId { get; init; }

    public string? UserEmail { get; init; }

    public required string EntityType { get; init; }

    public required string EntityId { get; init; }

    public required string Action { get; init; }

    public string? Details { get; init; }
}
