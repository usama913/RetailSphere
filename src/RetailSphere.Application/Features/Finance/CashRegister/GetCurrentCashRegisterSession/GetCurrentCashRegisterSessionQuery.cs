using MediatR;
using RetailSphere.Contracts.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.CashRegister.GetCurrentCashRegisterSession;

/// <summary>Returns null (inside a successful Result) when there is no open session for the branch — that's a normal state, not an error.</summary>
public sealed record GetCurrentCashRegisterSessionQuery(long BranchId) : IRequest<Result<CashRegisterSessionDto?>>;
