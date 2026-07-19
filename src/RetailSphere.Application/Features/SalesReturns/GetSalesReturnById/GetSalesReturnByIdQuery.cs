using MediatR;
using RetailSphere.Contracts.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesReturns.GetSalesReturnById;

public sealed record GetSalesReturnByIdQuery(long Id) : IRequest<Result<SalesReturnDto>>;
