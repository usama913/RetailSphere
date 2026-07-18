using MediatR;
using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.AuditLogs.GetAuditLogs;

public sealed record GetAuditLogsQuery(
    int Page,
    int PageSize,
    string? EntityType,
    string? Action,
    long? UserId,
    DateTime? FromUtc,
    DateTime? ToUtc) : IRequest<Result<PagedResult<AuditLogDto>>>;
