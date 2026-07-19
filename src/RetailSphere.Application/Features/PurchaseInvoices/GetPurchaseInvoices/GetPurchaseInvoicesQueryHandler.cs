using MediatR;
using RetailSphere.Application.Features.PurchaseInvoices.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.GetPurchaseInvoices;

public sealed class GetPurchaseInvoicesQueryHandler(IPurchaseInvoiceRepository purchaseInvoiceRepository, PurchaseInvoiceDtoAssembler purchaseInvoiceDtoAssembler)
    : IRequestHandler<GetPurchaseInvoicesQuery, Result<PagedResult<PurchaseInvoiceDto>>>
{
    public async Task<Result<PagedResult<PurchaseInvoiceDto>>> Handle(GetPurchaseInvoicesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await purchaseInvoiceRepository.SearchAsync(
            request.Page, request.PageSize, request.SupplierId, request.BranchId, request.PaymentStatus, request.FromDate, request.ToDate, cancellationToken);

        var dtos = await purchaseInvoiceDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<PurchaseInvoiceDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
