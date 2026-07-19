using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Finance;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<CashRegisterSessionDto?>> GetCurrentCashRegisterSessionAsync(long branchId, CancellationToken cancellationToken = default) =>
        GetAsync<CashRegisterSessionDto?>($"cash-register/current?branchId={branchId}", cancellationToken);

    public Task<ApiResponse<PagedResult<CashRegisterSessionDto>>> GetCashRegisterSessionsAsync(
        int page = 1,
        int pageSize = 25,
        long? branchId = null,
        string? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = $"page={page}&pageSize={pageSize}";

        if (branchId.HasValue)
            query += $"&branchId={branchId.Value}";

        if (!string.IsNullOrWhiteSpace(status))
            query += $"&status={Uri.EscapeDataString(status)}";

        return GetAsync<PagedResult<CashRegisterSessionDto>>($"cash-register?{query}", cancellationToken);
    }

    public Task<ApiResponse<CashRegisterSessionDto>> OpenCashRegisterSessionAsync(OpenCashRegisterSessionRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<OpenCashRegisterSessionRequest, CashRegisterSessionDto>("cash-register/open", request, cancellationToken);

    public Task<ApiResponse<CashRegisterSessionDto>> CloseCashRegisterSessionAsync(long id, CloseCashRegisterSessionRequest request, CancellationToken cancellationToken = default) =>
        PostAsync<CloseCashRegisterSessionRequest, CashRegisterSessionDto>($"cash-register/{id}/close", request, cancellationToken);
}
