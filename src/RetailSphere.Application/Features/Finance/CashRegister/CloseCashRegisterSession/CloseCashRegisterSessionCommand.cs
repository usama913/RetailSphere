using MediatR;
using RetailSphere.Contracts.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.CashRegister.CloseCashRegisterSession;

public sealed record CloseCashRegisterSessionCommand(
    long Id,
    decimal ClosingBalance,
    string? ClosingNotes) : IRequest<Result<CashRegisterSessionDto>>;
