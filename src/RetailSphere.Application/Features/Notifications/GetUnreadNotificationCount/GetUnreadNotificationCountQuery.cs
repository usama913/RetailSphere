using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Notifications.GetUnreadNotificationCount;

public sealed record GetUnreadNotificationCountQuery : IRequest<Result<int>>;
