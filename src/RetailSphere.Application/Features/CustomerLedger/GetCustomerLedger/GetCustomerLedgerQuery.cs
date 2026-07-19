using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerLedger.GetCustomerLedger;

public sealed record GetCustomerLedgerQuery(long CustomerId) : IRequest<Result<CustomerLedgerDto>>;
