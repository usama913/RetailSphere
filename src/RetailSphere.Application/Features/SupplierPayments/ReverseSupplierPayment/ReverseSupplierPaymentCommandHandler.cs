using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.SupplierPayments.Common;
using RetailSphere.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.ReverseSupplierPayment;

public sealed class ReverseSupplierPaymentCommandHandler(
    ISupplierPaymentRepository supplierPaymentRepository,
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    SupplierPaymentDtoAssembler supplierPaymentDtoAssembler,
    ICurrentUserService currentUserService)
    : IRequestHandler<ReverseSupplierPaymentCommand, Result<SupplierPaymentDto>>
{
    public async Task<Result<SupplierPaymentDto>> Handle(ReverseSupplierPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await supplierPaymentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null)
            return Result.Failure<SupplierPaymentDto>(Error.NotFound("SupplierPayment.NotFound", "Supplier payment not found."));

        var invoice = await purchaseInvoiceRepository.GetByIdAsync(payment.PurchaseInvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure<SupplierPaymentDto>(Error.NotFound("PurchaseInvoice.NotFound", "The invoice linked to this payment no longer exists."));

        var reverseResult = payment.Reverse(request.Reason, currentUserService.UserId);
        if (reverseResult.IsFailure)
            return Result.Failure<SupplierPaymentDto>(reverseResult.Error);

        var invoiceReverseResult = invoice.ReversePayment(payment.Amount);
        if (invoiceReverseResult.IsFailure)
            return Result.Failure<SupplierPaymentDto>(invoiceReverseResult.Error);

        supplierPaymentRepository.Update(payment);
        purchaseInvoiceRepository.Update(invoice);
        auditLogService.Log(
            "SupplierPayment", payment.Id.ToString(), "Reversed",
            $"Reversed payment of {payment.Amount:0.00} against invoice '{invoice.SupplierInvoiceNumber}' — reason: {request.Reason}");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await supplierPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
