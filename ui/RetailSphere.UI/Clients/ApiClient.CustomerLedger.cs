using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Customers;

namespace RetailSphere.UI.Clients;

public sealed partial class ApiClient
{
    public Task<ApiResponse<CustomerLedgerDto>> GetCustomerLedgerAsync(long customerId, CancellationToken cancellationToken = default) =>
        GetAsync<CustomerLedgerDto>($"customers/{customerId}/ledger", cancellationToken);

    public Task<ApiResponse<CustomerAgingReportDto>> GetCustomerAgingReportAsync(CancellationToken cancellationToken = default) =>
        GetAsync<CustomerAgingReportDto>("customers/aging-report", cancellationToken);

    public Task<ApiResponse<CustomerCreditSummaryDto>> GetCustomerCreditSummaryAsync(long customerId, CancellationToken cancellationToken = default) =>
        GetAsync<CustomerCreditSummaryDto>($"customers/{customerId}/credit-summary", cancellationToken);
}
