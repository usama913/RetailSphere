using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.CashRegister.GetCashRegisterSessions;

public sealed record GetCashRegisterSessionsQuery(
    int Page,
    int PageSize,
    long? BranchId,
    string? Status) : IRequest<Result<PagedResult<CashRegisterSessionDto>>>;
