using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.RecordCustomerPayment;

public sealed record RecordCustomerPaymentAllocationInput(long SalesOrderId, decimal Amount);

public sealed record RecordCustomerPaymentCommand(
    long CustomerId,
    long BranchId,
    DateTime PaymentDate,
    decimal Amount,
    string? PaymentMethod,
    string? ReferenceNumber,
    string? Notes,
    IReadOnlyList<RecordCustomerPaymentAllocationInput> Allocations) : IRequest<Result<CustomerPaymentDto>>;
