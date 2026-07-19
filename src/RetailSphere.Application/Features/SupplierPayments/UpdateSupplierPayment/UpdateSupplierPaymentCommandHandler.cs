using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.SupplierPayments.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.UpdateSupplierPayment;

/// <summary>
/// Editing a payment's Amount must keep its linked PurchaseInvoice's AmountPaid in
/// sync — applies the delta (new - old) onto the invoice within this same request,
/// exactly the same "two aggregates, one unit of work" discipline as RecordSupplierPayment.
/// </summary>
public sealed class UpdateSupplierPaymentCommandHandler(
    ISupplierPaymentRepository supplierPaymentRepository,
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    SupplierPaymentDtoAssembler supplierPaymentDtoAssembler)
    : IRequestHandler<UpdateSupplierPaymentCommand, Result<SupplierPaymentDto>>
{
    public async Task<Result<SupplierPaymentDto>> Handle(UpdateSupplierPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await supplierPaymentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payment is null)
            return Result.Failure<SupplierPaymentDto>(Error.NotFound("SupplierPayment.NotFound", "Supplier payment not found."));

        var invoice = await purchaseInvoiceRepository.GetByIdAsync(payment.PurchaseInvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure<SupplierPaymentDto>(Error.NotFound("PurchaseInvoice.NotFound", "The invoice linked to this payment no longer exists."));

        var updateResult = payment.UpdateDetails(request.PaymentDate, request.Amount, request.PaymentMethod, request.ReferenceNumber, request.Notes);
        if (updateResult.IsFailure)
            return Result.Failure<SupplierPaymentDto>(updateResult.Error);

        var previousAmount = updateResult.Value;
        var delta = request.Amount - previousAmount;

        if (delta > 0)
        {
            var applyResult = invoice.ApplyPayment(delta);
            if (applyResult.IsFailure)
                return Result.Failure<SupplierPaymentDto>(applyResult.Error);
        }
        else if (delta < 0)
        {
            var reverseResult = invoice.ReversePayment(-delta);
            if (reverseResult.IsFailure)
                return Result.Failure<SupplierPaymentDto>(reverseResult.Error);
        }

        supplierPaymentRepository.Update(payment);
        purchaseInvoiceRepository.Update(invoice);
        auditLogService.Log(
            "SupplierPayment", payment.Id.ToString(), "Updated",
            $"Edited payment against invoice '{invoice.SupplierInvoiceNumber}' from {previousAmount:0.00} to {payment.Amount:0.00}.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await supplierPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
