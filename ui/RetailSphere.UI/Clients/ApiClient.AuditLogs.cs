using RetailSphere.Contracts.Admin;
using RetailSphere.Contracts.Common;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<PagedResult<AuditLogDto>>> GetAuditLogsAsync(
        int page = 1,
        int pageSize = 25,
        string? entityType = null,
        string? action = null,
        long? userId = null,
        DateTime? fromUtc = null,
        DateTime? toUtc = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (!string.IsNullOrWhiteSpace(entityType))
            query += $"&entityType={Uri.EscapeDataString(entityType)}";

        if (!string.IsNullOrWhiteSpace(action))
            query += $"&action={Uri.EscapeDataString(action)}";

        if (userId.HasValue)
            query += $"&userId={userId.Value}";

        if (fromUtc.HasValue)
            query += $"&fromUtc={Uri.EscapeDataString(fromUtc.Value.ToString("O"))}";

        if (toUtc.HasValue)
            query += $"&toUtc={Uri.EscapeDataString(toUtc.Value.ToString("O"))}";

        return GetAsync<PagedResult<AuditLogDto>>($"audit-logs?{query}", cancellationToken);
    }
}
