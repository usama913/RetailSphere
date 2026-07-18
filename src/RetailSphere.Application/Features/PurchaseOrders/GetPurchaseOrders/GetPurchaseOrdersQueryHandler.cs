using MediatR;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.GetPurchaseOrders;

public sealed class GetPurchaseOrdersQueryHandler(IPurchaseOrderRepository purchaseOrderRepository, PurchaseOrderDtoAssembler purchaseOrderDtoAssembler)
    : IRequestHandler<GetPurchaseOrdersQuery, Result<PagedResult<PurchaseOrderDto>>>
{
    public async Task<Result<PagedResult<PurchaseOrderDto>>> Handle(GetPurchaseOrdersQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await purchaseOrderRepository.SearchAsync(
            request.Page, request.PageSize, request.Search, request.SupplierId, request.BranchId, request.Status, cancellationToken);

        var dtos = await purchaseOrderDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<PurchaseOrderDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
