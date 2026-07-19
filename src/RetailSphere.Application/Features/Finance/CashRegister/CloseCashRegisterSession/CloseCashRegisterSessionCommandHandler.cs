using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Finance.CashRegister.Common;
using RetailSphere.Common;
using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.CashRegister.CloseCashRegisterSession;

public sealed class CloseCashRegisterSessionCommandHandler(
    ICashRegisterSessionRepository cashRegisterSessionRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    CashRegisterSessionDtoAssembler cashRegisterSessionDtoAssembler,
    ICurrentUserService currentUserService)
    : IRequestHandler<CloseCashRegisterSessionCommand, Result<CashRegisterSessionDto>>
{
    public async Task<Result<CashRegisterSessionDto>> Handle(CloseCashRegisterSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await cashRegisterSessionRepository.GetByIdAsync(request.Id, cancellationToken);
        if (session is null)
            return Result.Failure<CashRegisterSessionDto>(Error.NotFound("CashRegisterSession.NotFound", "Cash register session not found."));

        var closeResult = session.Close(currentUserService.UserId ?? 0, request.ClosingBalance, request.ClosingNotes);
        if (closeResult.IsFailure)
            return Result.Failure<CashRegisterSessionDto>(closeResult.Error);

        cashRegisterSessionRepository.Update(session);
        auditLogService.Log("CashRegisterSession", session.Id.ToString(), "Closed", $"Closed cash register with closing balance {session.ClosingBalance:0.00}.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await cashRegisterSessionDtoAssembler.ToDtoWithTotalsAsync(session, cancellationToken);
        return Result.Success(dto);
    }
}
