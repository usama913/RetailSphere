using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerLedger.GetCustomerAgingReport;

public sealed record GetCustomerAgingReportQuery : IRequest<Result<CustomerAgingReportDto>>;
