using RetailSphere.Contracts.Notifications;
using RetailSphere.Domain.Notifications;

namespace RetailSphere.Application.Features.Notifications;

internal static class NotificationMappings
{
    public static NotificationDto ToDto(Notification notification) => new()
    {
        Id = notification.Id,
        Type = notification.Type,
        Severity = notification.Severity,
        Message = notification.Message,
        RelatedEntityType = notification.RelatedEntityType,
        RelatedEntityId = notification.RelatedEntityId,
        IsRead = notification.IsRead,
        ReadAtUtc = notification.ReadAtUtc,
        CreatedAtUtc = notification.CreatedAtUtc,
    };
}
