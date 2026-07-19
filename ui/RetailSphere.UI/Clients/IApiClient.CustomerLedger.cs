using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Customers;

namespace RetailSphere.UI.Clients;

public partial interface IApiClient
{
    Task<ApiResponse<CustomerLedgerDto>> GetCustomerLedgerAsync(long customerId, CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerAgingReportDto>> GetCustomerAgingReportAsync(CancellationToken cancellationToken = default);

    Task<ApiResponse<CustomerCreditSummaryDto>> GetCustomerCreditSummaryAsync(long customerId, CancellationToken cancellationToken = default);
}
