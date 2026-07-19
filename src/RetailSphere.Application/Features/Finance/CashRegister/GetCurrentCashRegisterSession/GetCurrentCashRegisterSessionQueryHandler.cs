using MediatR;
using RetailSphere.Application.Features.Finance.CashRegister.Common;
using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.CashRegister.GetCurrentCashRegisterSession;

public sealed class GetCurrentCashRegisterSessionQueryHandler(
    ICashRegisterSessionRepository cashRegisterSessionRepository,
    CashRegisterSessionDtoAssembler cashRegisterSessionDtoAssembler)
    : IRequestHandler<GetCurrentCashRegisterSessionQuery, Result<CashRegisterSessionDto?>>
{
    public async Task<Result<CashRegisterSessionDto?>> Handle(GetCurrentCashRegisterSessionQuery request, CancellationToken cancellationToken)
    {
        var session = await cashRegisterSessionRepository.GetOpenSessionAsync(request.BranchId, cancellationToken);
        if (session is null)
            return Result.Success<CashRegisterSessionDto?>(null);

        var dto = await cashRegisterSessionDtoAssembler.ToDtoWithTotalsAsync(session, cancellationToken);
        return Result.Success<CashRegisterSessionDto?>(dto);
    }
}
