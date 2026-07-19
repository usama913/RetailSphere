using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Organization;
using RetailSphere.Domain.Sales;

namespace RetailSphere.Application.Features.Finance.CashRegister.Common;

/// <summary>
/// Resolves BranchName/OpenedByUserName/ClosedByUserName lookups, and — only when asked
/// for via <see cref="ToDtoWithTotalsAsync"/> — the live cash-balance figures (a capped
/// bulk fetch of this session's Completed cash sales/cash expenses, summed client-side;
/// same accepted simplification as the Inventory Overview summary cards). History list
/// rows use the cheap <see cref="ToDtoAsync"/> overload and leave those fields null.
/// </summary>
public sealed class CashRegisterSessionDtoAssembler(
    IBranchRepository branchRepository,
    IUserRepository userRepository,
    ISalesOrderRepository salesOrderRepository,
    IExpenseRepository expenseRepository)
{
    private const int BulkFetchPageSize = 5000;

    public async Task<CashRegisterSessionDto> ToDtoAsync(CashRegisterSession session, CancellationToken cancellationToken = default)
    {
        var (branchName, openedByName, closedByName) = await ResolveNamesAsync(session, cancellationToken);
        return CashRegisterSessionMappings.ToDto(session, branchName, openedByName, closedByName);
    }

    public async Task<CashRegisterSessionDto> ToDtoWithTotalsAsync(CashRegisterSession session, CancellationToken cancellationToken = default)
    {
        var (branchName, openedByName, closedByName) = await ResolveNamesAsync(session, cancellationToken);

        var periodEnd = session.ClosedAtUtc ?? DateTime.UtcNow;

        var (salesOrders, _) = await salesOrderRepository.SearchAsync(
            page: 1, pageSize: BulkFetchPageSize, search: null, branchId: session.BranchId, customerId: null,
            status: "Completed", fromDate: session.OpenedAtUtc, toDate: periodEnd, cancellationToken);
        var totalCashSales = salesOrders.Where(o => o.PaymentMethod == "Cash").Sum(o => o.TotalAmount);

        var (expenses, _) = await expenseRepository.SearchAsync(
            page: 1, pageSize: BulkFetchPageSize, branchId: session.BranchId,
            fromDate: session.OpenedAtUtc, toDate: periodEnd, category: null, cancellationToken);
        var totalCashExpenses = expenses.Where(e => e.PaidFromCash).Sum(e => e.Amount);

        return CashRegisterSessionMappings.ToDto(session, branchName, openedByName, closedByName, totalCashSales, totalCashExpenses);
    }

    public async Task<IReadOnlyList<CashRegisterSessionDto>> ToDtosAsync(IEnumerable<CashRegisterSession> sessions, CancellationToken cancellationToken = default)
    {
        var items = sessions.ToList();

        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);

        var userIds = items.Select(s => s.OpenedByUserId)
            .Concat(items.Where(s => s.ClosedByUserId.HasValue).Select(s => s.ClosedByUserId!.Value))
            .Distinct()
            .ToList();
        var userNames = new Dictionary<long, string>();
        foreach (var userId in userIds)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is not null)
                userNames[userId] = FormatUserName(user)!;
        }

        return items
            .Select(session => CashRegisterSessionMappings.ToDto(
                session,
                branches.TryGetValue(session.BranchId, out var branchName) ? branchName : null,
                userNames.TryGetValue(session.OpenedByUserId, out var openedByName) ? openedByName : null,
                session.ClosedByUserId.HasValue && userNames.TryGetValue(session.ClosedByUserId.Value, out var closedByName) ? closedByName : null))
            .ToList();
    }

    private async Task<(string? BranchName, string? OpenedByName, string? ClosedByName)> ResolveNamesAsync(CashRegisterSession session, CancellationToken cancellationToken)
    {
        var branch = await branchRepository.GetByIdAsync(session.BranchId, cancellationToken);
        var openedBy = await userRepository.GetByIdAsync(session.OpenedByUserId, cancellationToken);
        var closedBy = session.ClosedByUserId.HasValue ? await userRepository.GetByIdAsync(session.ClosedByUserId.Value, cancellationToken) : null;

        return (branch?.Name, FormatUserName(openedBy), FormatUserName(closedBy));
    }

    private static string? FormatUserName(User? user) => user is null ? null : $"{user.FirstName} {user.LastName}";
}
