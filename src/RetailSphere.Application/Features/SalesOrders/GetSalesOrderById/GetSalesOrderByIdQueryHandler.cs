using MediatR;
using RetailSphere.Application.Features.SalesOrders.Common;
using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesOrders.GetSalesOrderById;

public sealed class GetSalesOrderByIdQueryHandler(ISalesOrderRepository salesOrderRepository, SalesOrderDtoAssembler salesOrderDtoAssembler)
    : IRequestHandler<GetSalesOrderByIdQuery, Result<SalesOrderDto>>
{
    public async Task<Result<SalesOrderDto>> Handle(GetSalesOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var salesOrder = await salesOrderRepository.GetByIdAsync(request.Id, cancellationToken);
        if (salesOrder is null)
            return Result.Failure<SalesOrderDto>(Error.NotFound("SalesOrder.NotFound", "Sales order not found."));

        var dto = await salesOrderDtoAssembler.ToDtoAsync(salesOrder, cancellationToken);
        return Result.Success(dto);
    }
}
