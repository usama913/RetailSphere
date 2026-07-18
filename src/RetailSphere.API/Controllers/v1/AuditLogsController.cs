using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.AuditLogs.GetAuditLogs;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/audit-logs")]
[Authorize]
public sealed class AuditLogsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "admin.audit.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? entityType,
        [FromQuery] string? action,
        [FromQuery] long? userId,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetAuditLogsQuery(effectivePage, effectivePageSize, entityType, action, userId, fromUtc, toUtc),
            cancellationToken);
        return HandleResult(result);
    }
}
