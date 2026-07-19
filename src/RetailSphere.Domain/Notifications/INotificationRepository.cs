namespace RetailSphere.Domain.Notifications;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>Notifications visible to a user — their own (UserId == userId) plus every broadcast alert (UserId == null), newest first.</summary>
    Task<(IReadOnlyList<Notification> Items, long TotalCount)> GetForUserAsync(
        long userId,
        bool unreadOnly,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountForUserAsync(long userId, CancellationToken cancellationToken = default);

    /// <summary>Every notification not yet emailed — polled by the Hangfire notification sweep job.</summary>
    Task<IReadOnlyList<Notification>> GetPendingEmailAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Used by the daily sweep to avoid raising a duplicate "still overdue" alert every
    /// day for the same document — checks whether a notification of this type already
    /// exists for this related entity, generated after the given cutoff.
    /// </summary>
    Task<bool> ExistsForEntitySinceAsync(string type, string relatedEntityType, long relatedEntityId, DateTime sinceUtc, CancellationToken cancellationToken = default);

    void Add(Notification notification);

    void Update(Notification notification);
}
