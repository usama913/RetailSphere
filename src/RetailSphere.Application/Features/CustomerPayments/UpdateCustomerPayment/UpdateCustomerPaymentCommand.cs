using MediatR;
using RetailSphere.Contracts.Customers;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.CustomerPayments.UpdateCustomerPayment;

public sealed record UpdateCustomerPaymentCommand(
    long Id,
    DateTime PaymentDate,
    decimal Amount,
    string? PaymentMethod,
    string? ReferenceNumber,
    string? Notes) : IRequest<Result<CustomerPaymentDto>>;
