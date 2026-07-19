using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.DeletePurchaseInvoice;

public sealed class DeletePurchaseInvoiceCommandHandler(
    IPurchaseInvoiceRepository purchaseInvoiceRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeletePurchaseInvoiceCommand, Result>
{
    public async Task<Result> Handle(DeletePurchaseInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoice = await purchaseInvoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice is null)
            return Result.Failure(Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found."));

        if (invoice.AmountPaid > 0)
            return Result.Failure(Error.Conflict("PurchaseInvoice.HasPayments", "This invoice already has payments recorded — reverse them before deleting it."));

        purchaseInvoiceRepository.Remove(invoice);
        auditLogService.Log("PurchaseInvoice", invoice.Id.ToString(), "Deleted", $"Deleted purchase invoice '{invoice.SupplierInvoiceNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
