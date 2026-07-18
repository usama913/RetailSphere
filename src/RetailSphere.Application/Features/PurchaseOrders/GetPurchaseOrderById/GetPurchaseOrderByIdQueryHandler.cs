using MediatR;
using RetailSphere.Application.Features.PurchaseOrders.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseOrders.GetPurchaseOrderById;

public sealed class GetPurchaseOrderByIdQueryHandler(IPurchaseOrderRepository purchaseOrderRepository, PurchaseOrderDtoAssembler purchaseOrderDtoAssembler)
    : IRequestHandler<GetPurchaseOrderByIdQuery, Result<PurchaseOrderDto>>
{
    public async Task<Result<PurchaseOrderDto>> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var purchaseOrder = await purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (purchaseOrder is null)
            return Result.Failure<PurchaseOrderDto>(Error.NotFound("PurchaseOrder.NotFound", "Purchase order not found."));

        var dto = await purchaseOrderDtoAssembler.ToDtoAsync(purchaseOrder, cancellationToken);
        return Result.Success(dto);
    }
}
