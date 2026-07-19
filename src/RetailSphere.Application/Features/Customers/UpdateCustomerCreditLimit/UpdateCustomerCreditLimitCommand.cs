using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Customers.UpdateCustomerCreditLimit;

public sealed record UpdateCustomerCreditLimitCommand(long Id, decimal? CreditLimit) : IRequest<Result<CustomerDto>>;
