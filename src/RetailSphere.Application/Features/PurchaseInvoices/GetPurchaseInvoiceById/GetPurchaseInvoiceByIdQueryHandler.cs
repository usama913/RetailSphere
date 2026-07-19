using MediatR;
using RetailSphere.Application.Features.PurchaseInvoices.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.GetPurchaseInvoiceById;

public sealed class GetPurchaseInvoiceByIdQueryHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository, PurchaseInvoiceDtoAssembler purchaseInvoiceDtoAssembler)
    : IRequestHandler<GetPurchaseInvoiceByIdQuery, Result<PurchaseInvoiceDto>>
{
    public async Task<Result<PurchaseInvoiceDto>> Handle(GetPurchaseInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await purchaseInvoiceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (invoice is null)
            return Result.Failure<PurchaseInvoiceDto>(Error.NotFound("PurchaseInvoice.NotFound", "Purchase invoice not found."));

        var dto = await purchaseInvoiceDtoAssembler.ToDtoAsync(invoice, cancellationToken);
        return Result.Success(dto);
    }
}
