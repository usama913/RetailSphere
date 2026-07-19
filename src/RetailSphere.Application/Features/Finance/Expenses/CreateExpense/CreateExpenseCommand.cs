using MediatR;
using RetailSphere.Contracts.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.CreateExpense;

public sealed record CreateExpenseCommand(
    long BranchId,
    DateTime ExpenseDate,
    decimal Amount,
    string? Category,
    string? Description,
    bool PaidFromCash) : IRequest<Result<ExpenseDto>>;
