using MediatR;
using RetailSphere.Application.Features.Finance.CashRegister.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.CashRegister.GetCashRegisterSessions;

public sealed class GetCashRegisterSessionsQueryHandler(
    ICashRegisterSessionRepository cashRegisterSessionRepository,
    CashRegisterSessionDtoAssembler cashRegisterSessionDtoAssembler)
    : IRequestHandler<GetCashRegisterSessionsQuery, Result<PagedResult<CashRegisterSessionDto>>>
{
    public async Task<Result<PagedResult<CashRegisterSessionDto>>> Handle(GetCashRegisterSessionsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await cashRegisterSessionRepository.SearchAsync(
            request.Page, request.PageSize, request.BranchId, request.Status, cancellationToken);

        var dtos = await cashRegisterSessionDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<CashRegisterSessionDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
