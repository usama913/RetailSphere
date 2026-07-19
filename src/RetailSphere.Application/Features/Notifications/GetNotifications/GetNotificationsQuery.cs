using MediatR;
using RetailSphere.Contracts.Notifications;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Notifications.GetNotifications;

public sealed record GetNotificationsQuery(bool UnreadOnly, int Page, int PageSize) : IRequest<Result<NotificationListDto>>;
