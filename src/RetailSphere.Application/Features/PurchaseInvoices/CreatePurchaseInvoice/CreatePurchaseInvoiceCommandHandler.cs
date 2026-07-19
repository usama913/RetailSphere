using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.PurchaseInvoices.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.CreatePurchaseInvoice;

public sealed class CreatePurchaseInvoiceCommandHandler(
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    PurchaseInvoiceDtoAssembler purchaseInvoiceDtoAssembler)
    : IRequestHandler<CreatePurchaseInvoiceCommand, Result<PurchaseInvoiceDto>>
{
    public async Task<Result<PurchaseInvoiceDto>> Handle(CreatePurchaseInvoiceCommand request, CancellationToken cancellationToken)
    {
        var alreadyExists = await purchaseInvoiceRepository.SupplierInvoiceNumberExistsAsync(
            request.SupplierId, request.SupplierInvoiceNumber, excludeId: null, cancellationToken);
        if (alreadyExists)
            return Result.Failure<PurchaseInvoiceDto>(Error.Conflict("PurchaseInvoice.DuplicateInvoiceNumber", "This supplier already has an invoice recorded with that number."));

        var invoiceResult = PurchaseInvoice.Create(
            request.SupplierId,
            request.BranchId,
            request.PurchaseOrderId,
            request.SupplierInvoiceNumber,
            request.InvoiceDate,
            request.DueDate,
            request.PaymentTerms,
            request.SubtotalAmount,
            request.DiscountAmount,
            request.TaxAmount,
            request.Notes);
        if (invoiceResult.IsFailure)
            return Result.Failure<PurchaseInvoiceDto>(invoiceResult.Error);

        var invoice = invoiceResult.Value;
        purchaseInvoiceRepository.Add(invoice);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("PurchaseInvoice", invoice.Id.ToString(), "Created", $"Recorded purchase invoice '{invoice.SupplierInvoiceNumber}' for {invoice.TotalAmount:0.00}.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await purchaseInvoiceDtoAssembler.ToDtoAsync(invoice, cancellationToken);
        return Result.Success(dto);
    }
}
