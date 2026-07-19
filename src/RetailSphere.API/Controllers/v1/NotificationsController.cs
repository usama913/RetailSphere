using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Notifications.GetNotifications;
using RetailSphere.Application.Features.Notifications.GetUnreadNotificationCount;
using RetailSphere.Application.Features.Notifications.MarkNotificationAsRead;

namespace RetailSphere.API.Controllers.v1;

/// <summary>
/// Notifications are personal (each authenticated user sees their own bell), so
/// unlike most controllers here there's no permission-code gate beyond being
/// logged in — [Authorize] on the class is enough.
/// </summary>
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[Authorize]
public sealed class NotificationsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool unreadOnly, [FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(new GetNotificationsQuery(unreadOnly, effectivePage, effectivePageSize), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetUnreadNotificationCountQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/mark-read")]
    public async Task<IActionResult> MarkAsRead(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new MarkNotificationAsReadCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
