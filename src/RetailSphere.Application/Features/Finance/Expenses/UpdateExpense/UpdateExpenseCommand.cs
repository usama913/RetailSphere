using MediatR;
using RetailSphere.Contracts.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.UpdateExpense;

public sealed record UpdateExpenseCommand(
    long Id,
    DateTime ExpenseDate,
    decimal Amount,
    string? Category,
    string? Description,
    bool PaidFromCash) : IRequest<Result<ExpenseDto>>;
