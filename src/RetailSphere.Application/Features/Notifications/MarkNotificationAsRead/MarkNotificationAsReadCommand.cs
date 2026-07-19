using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Notifications.MarkNotificationAsRead;

public sealed record MarkNotificationAsReadCommand(long Id) : IRequest<Result>;
