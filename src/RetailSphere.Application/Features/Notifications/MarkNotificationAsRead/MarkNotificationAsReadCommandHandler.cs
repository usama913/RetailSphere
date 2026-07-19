using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Domain.Notifications;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Notifications.MarkNotificationAsRead;

public sealed class MarkNotificationAsReadCommandHandler(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<MarkNotificationAsReadCommand, Result>
{
    public async Task<Result> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await notificationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (notification is null)
            return Result.Failure(Error.NotFound("Notification.NotFound", "Notification not found."));

        notification.MarkAsRead();
        notificationRepository.Update(notification);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
