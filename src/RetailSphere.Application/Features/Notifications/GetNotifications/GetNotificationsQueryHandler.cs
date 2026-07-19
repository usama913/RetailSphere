using MediatR;
using RetailSphere.Common;
using RetailSphere.Contracts.Notifications;
using RetailSphere.Domain.Notifications;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Notifications.GetNotifications;

public sealed class GetNotificationsQueryHandler(INotificationRepository notificationRepository, ICurrentUserService currentUserService)
    : IRequestHandler<GetNotificationsQuery, Result<NotificationListDto>>
{
    public async Task<Result<NotificationListDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.UserId.HasValue)
            return Result.Failure<NotificationListDto>(Error.Unauthorized("Notification.NotAuthenticated", "Not authenticated."));

        var userId = currentUserService.UserId.Value;

        var (items, _) = await notificationRepository.GetForUserAsync(userId, request.UnreadOnly, request.Page, request.PageSize, cancellationToken);
        var unreadCount = await notificationRepository.GetUnreadCountForUserAsync(userId, cancellationToken);

        return Result.Success(new NotificationListDto
        {
            Items = items.Select(NotificationMappings.ToDto).ToList(),
            UnreadCount = unreadCount,
        });
    }
}
