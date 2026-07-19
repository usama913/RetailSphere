using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.SupplierPayments.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.RecordSupplierPayment;

public sealed class RecordSupplierPaymentCommandHandler(
    ISupplierPaymentRepository supplierPaymentRepository,
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    SupplierPaymentDtoAssembler supplierPaymentDtoAssembler)
    : IRequestHandler<RecordSupplierPaymentCommand, Result<SupplierPaymentDto>>
{
    public async Task<Result<SupplierPaymentDto>> Handle(RecordSupplierPaymentCommand request, CancellationToken cancellationToken)
    {
        var invoice = await purchaseInvoiceRepository.GetByIdAsync(request.PurchaseInvoiceId, cancellationToken);
        if (invoice is null)
            return Result.Failure<SupplierPaymentDto>(Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found."));

        if (invoice.SupplierId != request.SupplierId)
            return Result.Failure<SupplierPaymentDto>(Error.Validation("SupplierPayment.SupplierMismatch", "This invoice does not belong to the specified supplier."));

        var paymentResult = SupplierPayment.Create(
            request.SupplierId, request.PurchaseInvoiceId, request.BranchId, request.PaymentDate, request.Amount, request.PaymentMethod, request.ReferenceNumber, request.Notes);
        if (paymentResult.IsFailure)
            return Result.Failure<SupplierPaymentDto>(paymentResult.Error);

        var applyResult = invoice.ApplyPayment(request.Amount);
        if (applyResult.IsFailure)
            return Result.Failure<SupplierPaymentDto>(applyResult.Error);

        var payment = paymentResult.Value;
        supplierPaymentRepository.Add(payment);
        purchaseInvoiceRepository.Update(invoice);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log(
            "SupplierPayment", payment.Id.ToString(), "Created",
            $"Recorded payment of {payment.Amount:0.00} against invoice '{invoice.SupplierInvoiceNumber}' — new outstanding balance {invoice.OutstandingBalance:0.00}.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await supplierPaymentDtoAssembler.ToDtoAsync(payment, cancellationToken);
        return Result.Success(dto);
    }
}
