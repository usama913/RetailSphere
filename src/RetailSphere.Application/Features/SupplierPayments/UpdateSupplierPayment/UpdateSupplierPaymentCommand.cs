using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.UpdateSupplierPayment;

public sealed record UpdateSupplierPaymentCommand(
    long Id,
    DateTime PaymentDate,
    decimal Amount,
    string? PaymentMethod,
    string? ReferenceNumber,
    string? Notes) : IRequest<Result<SupplierPaymentDto>>;
