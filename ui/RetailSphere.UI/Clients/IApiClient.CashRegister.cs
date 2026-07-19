using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Finance;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<CashRegisterSessionDto?>> GetCurrentCashRegisterSessionAsync(long branchId, CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResult<CashRegisterSessionDto>>> GetCashRegisterSessionsAsync(
        int page = 1,
        int pageSize = 25,
        long? branchId = null,
        string? status = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<CashRegisterSessionDto>> OpenCashRegisterSessionAsync(OpenCashRegisterSessionRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<CashRegisterSessionDto>> CloseCashRegisterSessionAsync(long id, CloseCashRegisterSessionRequest request, CancellationToken cancellationToken = default);
}
