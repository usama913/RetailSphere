using MediatR;
using RetailSphere.Application.Features.Finance.Expenses.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.GetExpenses;

public sealed class GetExpensesQueryHandler(IExpenseRepository expenseRepository, ExpenseDtoAssembler expenseDtoAssembler)
    : IRequestHandler<GetExpensesQuery, Result<PagedResult<ExpenseDto>>>
{
    public async Task<Result<PagedResult<ExpenseDto>>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await expenseRepository.SearchAsync(
            request.Page, request.PageSize, request.BranchId, request.FromDate, request.ToDate, request.Category, cancellationToken);

        var dtos = await expenseDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<ExpenseDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
