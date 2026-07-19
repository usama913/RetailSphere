using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;

namespace RetailSphere.Application.Features.Finance.CashRegister;

internal static class CashRegisterSessionMappings
{
    public static CashRegisterSessionDto ToDto(
        CashRegisterSession session,
        string? branchName,
        string? openedByUserName,
        string? closedByUserName,
        decimal? totalCashSales = null,
        decimal? totalCashExpenses = null) => new()
    {
        Id = session.Id,
        BranchId = session.BranchId,
        BranchName = branchName,
        OpenedByUserId = session.OpenedByUserId,
        OpenedByUserName = openedByUserName,
        ClosedByUserId = session.ClosedByUserId,
        ClosedByUserName = closedByUserName,
        Status = session.Status,
        OpeningBalance = session.OpeningBalance,
        ClosingBalance = session.ClosingBalance,
        OpenedAtUtc = session.OpenedAtUtc,
        ClosedAtUtc = session.ClosedAtUtc,
        OpeningNotes = session.OpeningNotes,
        ClosingNotes = session.ClosingNotes,
        TotalCashSales = totalCashSales,
        TotalCashExpenses = totalCashExpenses,
        CurrentCashBalance = totalCashSales.HasValue && totalCashExpenses.HasValue
            ? session.OpeningBalance + totalCashSales.Value - totalCashExpenses.Value
            : null,
    };
}
