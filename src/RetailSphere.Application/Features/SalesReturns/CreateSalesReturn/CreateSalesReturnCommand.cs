using MediatR;
using RetailSphere.Contracts.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesReturns.CreateSalesReturn;

public sealed record CreateSalesReturnLineItem(long SalesOrderLineId, decimal Quantity);

public sealed record CreateSalesReturnCommand(
    long SalesOrderId,
    string? Reason,
    IReadOnlyList<CreateSalesReturnLineItem> Lines) : IRequest<Result<SalesReturnDto>>;
