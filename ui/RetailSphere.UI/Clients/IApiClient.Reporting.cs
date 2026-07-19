using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Reporting;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<FinancialSummaryDto>> GetFinancialSummaryAsync(CancellationToken cancellationToken = default);
}
