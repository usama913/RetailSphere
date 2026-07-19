using MediatR;
using RetailSphere.Common;
using RetailSphere.Domain.Notifications;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Notifications.GetUnreadNotificationCount;

public sealed class GetUnreadNotificationCountQueryHandler(INotificationRepository notificationRepository, ICurrentUserService currentUserService)
    : IRequestHandler<GetUnreadNotificationCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(GetUnreadNotificationCountQuery request, CancellationToken cancellationToken)
    {
        if (!currentUserService.UserId.HasValue)
            return Result.Failure<int>(Error.Unauthorized("Notification.NotAuthenticated", "Not authenticated."));

        var count = await notificationRepository.GetUnreadCountForUserAsync(currentUserService.UserId.Value, cancellationToken);
        return Result.Success(count);
    }
}
