using Microsoft.EntityFrameworkCore;
using RetailSphere.Domain.Notifications;

namespace RetailSphere.Persistence.Repositories;

public sealed class NotificationRepository(RetailSphereDbContext dbContext) : INotificationRepository
{
    public Task<Notification?> GetByIdAsync(long id, CancellationToken cancellationToken = default) =>
        dbContext.Notifications.FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Notification> Items, long TotalCount)> GetForUserAsync(
        long userId,
        bool unreadOnly,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Notifications
            .Where(n => n.UserId == null || n.UserId == userId)
            .AsQueryable();

        if (unreadOnly)
            query = query.Where(n => !n.IsRead);

        query = query.OrderByDescending(n => n.CreatedAtUtc);

        var totalCount = await query.LongCountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<int> GetUnreadCountForUserAsync(long userId, CancellationToken cancellationToken = default) =>
        dbContext.Notifications.CountAsync(n => (n.UserId == null || n.UserId == userId) && !n.IsRead, cancellationToken);

    public async Task<IReadOnlyList<Notification>> GetPendingEmailAsync(CancellationToken cancellationToken = default) =>
        await dbContext.Notifications
            .Where(n => !n.EmailSent)
            .OrderBy(n => n.CreatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task<bool> ExistsForEntitySinceAsync(string type, string relatedEntityType, long relatedEntityId, DateTime sinceUtc, CancellationToken cancellationToken = default) =>
        dbContext.Notifications.AnyAsync(
            n => n.Type == type
                && n.RelatedEntityType == relatedEntityType
                && n.RelatedEntityId == relatedEntityId
                && n.CreatedAtUtc >= sinceUtc,
            cancellationToken);

    public void Add(Notification notification) => dbContext.Notifications.Add(notification);

    public void Update(Notification notification) => dbContext.Notifications.Update(notification);
}
