using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<PagedResult<AuditLogDto>>> GetAuditLogsAsync(
        int page = 1,
        int pageSize = 25,
        string? entityType = null,
        string? action = null,
        long? userId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default);
}
