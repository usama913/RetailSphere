using MediatR;
using RetailSphere.Contracts.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.CashRegister.OpenCashRegisterSession;

public sealed record OpenCashRegisterSessionCommand(
    long BranchId,
    decimal OpeningBalance,
    string? OpeningNotes) : IRequest<Result<CashRegisterSessionDto>>;
