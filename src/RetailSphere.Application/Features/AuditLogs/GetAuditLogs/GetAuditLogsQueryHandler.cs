using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;
using RetailSphere.Domain.Auditing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.AuditLogs.GetAuditLogs;

public sealed class GetAuditLogsQueryHandler(IAuditLogRepository auditLogRepository)
    : IRequestHandler<GetAuditLogsQuery, Result<PagedResult<AuditLogDto>>>
{
    public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await auditLogRepository.SearchAsync(
            request.Page, request.PageSize, request.EntityType, request.Action, request.UserId, request.FromUtc, request.ToUtc, cancellationToken);

        var dtos = items.Select(AuditLogMappings.ToDto).ToList();

        return Result.Success(new PagedResult<AuditLogDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
