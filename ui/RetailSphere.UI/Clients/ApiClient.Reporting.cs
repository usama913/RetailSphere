using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Reporting;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<FinancialSummaryDto>> GetFinancialSummaryAsync(CancellationToken cancellationToken = default) =>
        GetAsync<FinancialSummaryDto>("reporting/financial-summary", cancellationToken);
}
