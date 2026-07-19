using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.GetCustomerPaymentById;

public sealed record GetCustomerPaymentByIdQuery(long Id) : IRequest<Result<CustomerPaymentDto>>;
