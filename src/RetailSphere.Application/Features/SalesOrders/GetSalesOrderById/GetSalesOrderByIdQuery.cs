using MediatR;
using RetailSphere.Contracts.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesOrders.GetSalesOrderById;

public sealed record GetSalesOrderByIdQuery(long Id) : IRequest<Result<SalesOrderDto>>;
