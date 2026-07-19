using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.RecordSupplierPayment;

public sealed record RecordSupplierPaymentCommand(
    long SupplierId,
    long PurchaseInvoiceId,
    long BranchId,
    DateTime PaymentDate,
    decimal Amount,
    string? PaymentMethod,
    string? ReferenceNumber,
    string? Notes) : IRequest<Result<SupplierPaymentDto>>;
