using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.AllocateCustomerPayment;

public sealed record AllocateCustomerPaymentCommand(long PaymentId, long SalesOrderId, decimal Amount) : IRequest<Result<CustomerPaymentDto>>;
