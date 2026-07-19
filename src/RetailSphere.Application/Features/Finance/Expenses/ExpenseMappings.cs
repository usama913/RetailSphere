using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;

namespace RetailSphere.Application.Features.Finance.Expenses;

internal static class ExpenseMappings
{
    public static ExpenseDto ToDto(Expense expense, string? branchName, string? recordedByUserName) => new()
    {
        Id = expense.Id,
        BranchId = expense.BranchId,
        BranchName = branchName,
        ExpenseDate = expense.ExpenseDate,
        Amount = expense.Amount,
        Category = expense.Category,
        Description = expense.Description,
        PaidFromCash = expense.PaidFromCash,
        RecordedByUserId = expense.RecordedByUserId,
        RecordedByUserName = recordedByUserName,
    };
}
