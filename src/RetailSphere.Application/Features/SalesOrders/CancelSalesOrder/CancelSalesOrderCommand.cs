using MediatR;
using RetailSphere.Contracts.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesOrders.CancelSalesOrder;

public sealed record CancelSalesOrderCommand(long Id, string Reason) : IRequest<Result<SalesOrderDto>>;
