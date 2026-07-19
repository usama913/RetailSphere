namespace RetailSphere.Contracts.Notifications;

public sealed class NotificationDto
{
    public required long Id { get; init; }

    public required string Type { get; init; }

    public required string Severity { get; init; }

    public required string Message { get; init; }

    public string? RelatedEntityType { get; init; }

    public long? RelatedEntityId { get; init; }

    public required bool IsRead { get; init; }

    public DateTime? ReadAtUtc { get; init; }

    public required DateTime CreatedAtUtc { get; init; }
}

public sealed class NotificationListDto
{
    public required IReadOnlyList<NotificationDto> Items { get; init; }

    public required int UnreadCount { get; init; }
}
