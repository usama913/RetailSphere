using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.GetExpenses;

public sealed record GetExpensesQuery(
    int Page,
    int PageSize,
    long? BranchId,
    DateTime? FromDate,
    DateTime? ToDate,
    string? Category) : IRequest<Result<PagedResult<ExpenseDto>>>;
