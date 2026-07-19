using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.RemoveCustomerPaymentAllocation;

public sealed record RemoveCustomerPaymentAllocationCommand(long PaymentId, long SalesOrderId) : IRequest<Result<CustomerPaymentDto>>;
