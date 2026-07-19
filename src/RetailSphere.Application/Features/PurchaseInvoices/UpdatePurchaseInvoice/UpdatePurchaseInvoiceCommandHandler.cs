using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.PurchaseInvoices.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.UpdatePurchaseInvoice;

public sealed class UpdatePurchaseInvoiceCommandHandler(
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    PurchaseInvoiceDtoAssembler purchaseInvoiceDtoAssembler)
    : IRequestHandler<UpdatePurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    public async Task<Result<PurchaseInvoiceDto>> Handle(UpdatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await purchaseInvoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice is null)
            return Result.Failure<PurchaseInvoiceDto>(Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found."));

        var duplicateNumber = await purchaseInvoiceRepository.SupplierInvoiceNumberExistsAsync(
            invoice.SupplierId, request.SupplierInvoiceNumber, excludeId: invoice.Id, cancellationToken);
        if (duplicateNumber)
            return Result.Failure<PurchaseInvoiceDto>(Error.Conflict("PurchaseInvoice.DuplicateInvoiceNumber", "This supplier already has an invoice recorded with that number."));

        var updateResult = invoice.UpdateDetails(
            request.SupplierInvoiceNumber,
            request.InvoiceDate,
            request.DueDate,
            request.PaymentTerms,
            request.SubtotalAmount,
            request.DiscountAmount,
            request.TaxAmount,
            request.Notes);
        if (updateResult.IsFailure)
            return Result.Failure<PurchaseInvoiceDto>(updateResult.Error);

        purchaseInvoiceRepository.Update(invoice);
        auditLogService.Log("PurchaseInvoice", invoice.Id.ToString(), "Updated", $"Updated purchase invoice '{invoice.SupplierInvoiceNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await purchaseInvoiceDtoAssembler.ToDtoAsync(invoice, cancellationToken);
        return Result.Success(dto);
    }
}
