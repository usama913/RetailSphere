using MediatR;
using RetailSphere.Application.Features.SalesReturns.Common;
using RetailSphere.Contracts.Sales;
using RetailSphere.Domain.Sales;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SalesReturns.GetSalesReturnById;

public sealed class GetSalesReturnByIdQueryHandler(ISalesReturnRepository salesReturnRepository, SalesReturnDtoAssembler salesReturnDtoAssembler)
    : IRequestHandler<GetSalesReturnByIdQuery, Result<SalesReturnDto>>
{
    public async Task<Result<SalesReturnDto>> Handle(GetSalesReturnByIdQuery request, CancellationToken cancellationToken)
    {
        var salesReturn = await salesReturnRepository.GetByIdAsync(request.Id, cancellationToken);
        if (salesReturn is null)
            return Result.Failure<SalesReturnDto>(Error.NotFound("SalesReturn.NotFound", "Sales return not found."));

        var dto = await salesReturnDtoAssembler.ToDtoAsync(salesReturn, cancellationToken);
        return Result.Success(dto);
    }
}
