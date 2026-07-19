using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerLedger.GetCustomerCreditSummary;

public sealed record GetCustomerCreditSummaryQuery(long CustomerId) : IRequest<Result<CustomerCreditSummaryDto>>;
