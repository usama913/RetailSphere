using MediatR;
using RetailSphere.Domain.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.GetExpenseCategories;

/// <summary>
/// Merges the suggested default categories (Expense.Categories) with whatever
/// custom category names branches have actually recorded so far — Expense.Category
/// is freeform (see Expense.NormalizeCategory's remarks), so this is how a
/// newly-typed category becomes a real, selectable option for everyone from then on.
/// </summary>
public sealed class GetExpenseCategoriesQueryHandler(IExpenseRepository expenseRepository)
    : IRequestHandler<GetExpenseCategoriesQuery, Result<IReadOnlyList<string>>>
{
    public async Task<Result<IReadOnlyList<string>>> Handle(GetExpenseCategoriesQuery request, CancellationToken cancellationToken)
    {
        var recorded = await expenseRepository.GetDistinctCategoriesAsync(cancellationToken);

        var merged = Expense.Categories
            .Concat(recorded)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .GroupBy(c => c, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return Result.Success<IReadOnlyList<string>>(merged);
    }
}
