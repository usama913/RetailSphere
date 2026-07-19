using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;
using RetailSphere.Domain.IdentityAccess;
using RetailSphere.Domain.Organization;

namespace RetailSphere.Application.Features.Finance.Expenses.Common;

/// <summary>Resolves the BranchName/RecordedByUserName lookups ExpenseDto needs on top of Expense's own scalar fields — mirrors SalesOrderDtoAssembler.</summary>
public sealed class ExpenseDtoAssembler(IBranchRepository branchRepository, IUserRepository userRepository)
{
    public async Task<ExpenseDto> ToDtoAsync(Expense expense, CancellationToken cancellationToken = default)
    {
        var branch = await branchRepository.GetByIdAsync(expense.BranchId, cancellationToken);
        var recordedBy = expense.RecordedByUserId.HasValue ? await userRepository.GetByIdAsync(expense.RecordedByUserId.Value, cancellationToken) : null;

        return ExpenseMappings.ToDto(expense, branch?.Name, FormatUserName(recordedBy));
    }

    public async Task<IReadOnlyList<ExpenseDto>> ToDtosAsync(IEnumerable<Expense> expenses, CancellationToken cancellationToken = default)
    {
        var items = expenses.ToList();

        var branches = (await branchRepository.GetAllAsync(includeInactive: true, cancellationToken))
            .ToDictionary(b => b.Id, b => b.Name);

        var userIds = items.Where(e => e.RecordedByUserId.HasValue).Select(e => e.RecordedByUserId!.Value).Distinct().ToList();
        var userNames = new Dictionary<long, string>();
        foreach (var userId in userIds)
        {
            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is not null)
                userNames[userId] = FormatUserName(user)!;
        }

        return items
            .Select(expense => ExpenseMappings.ToDto(
                expense,
                branches.TryGetValue(expense.BranchId, out var branchName) ? branchName : null,
                expense.RecordedByUserId.HasValue && userNames.TryGetValue(expense.RecordedByUserId.Value, out var userName) ? userName : null))
            .ToList();
    }

    private static string? FormatUserName(User? user) => user is null ? null : $"{user.FirstName} {user.LastName}";
}
