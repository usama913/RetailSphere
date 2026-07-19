using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Finance.CashRegister.Common;
using RetailSphere.Common;
using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.CashRegister.OpenCashRegisterSession;

public sealed class OpenCashRegisterSessionCommandHandler(
    ICashRegisterSessionRepository cashRegisterSessionRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    CashRegisterSessionDtoAssembler cashRegisterSessionDtoAssembler,
    ICurrentUserService currentUserService)
    : IRequestHandler<OpenCashRegisterSessionCommand, Result<CashRegisterSessionDto>>
{
    public async Task<Result<CashRegisterSessionDto>> Handle(OpenCashRegisterSessionCommand request, CancellationToken cancellationToken)
    {
        var existingOpen = await cashRegisterSessionRepository.GetOpenSessionAsync(request.BranchId, cancellationToken);
        if (existingOpen is not null)
            return Result.Failure<CashRegisterSessionDto>(Error.Conflict("CashRegisterSession.AlreadyOpen", "There is already an open cash register session for this branch."));

        if (currentUserService.UserId is not { } userId)
            return Result.Failure<CashRegisterSessionDto>(Error.Validation("CashRegisterSession.UserRequired", "A signed-in user is required to open a cash register session."));

        var sessionResult = CashRegisterSession.Open(request.BranchId, userId, request.OpeningBalance, request.OpeningNotes);
        if (sessionResult.IsFailure)
            return Result.Failure<CashRegisterSessionDto>(sessionResult.Error);

        var session = sessionResult.Value;
        cashRegisterSessionRepository.Add(session);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("CashRegisterSession", session.Id.ToString(), "Opened", $"Opened cash register with opening balance {session.OpeningBalance:0.00}.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await cashRegisterSessionDtoAssembler.ToDtoWithTotalsAsync(session, cancellationToken);
        return Result.Success(dto);
    }
}
