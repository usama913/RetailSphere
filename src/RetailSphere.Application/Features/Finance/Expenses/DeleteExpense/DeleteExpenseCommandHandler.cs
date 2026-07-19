using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.DeleteExpense;

public sealed class DeleteExpenseCommandHandler(
    IExpenseRepository expenseRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeleteExpenseCommand, Result>
{
    public async Task<Result> Handle(DeleteExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await expenseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (expense is null)
            return Result.Failure(Error.NotFound("Expense.NotFound", "Expense not found."));

        expenseRepository.Remove(expense);
        auditLogService.Log("Expense", expense.Id.ToString(), "Deleted", $"Deleted expense of {expense.Amount:0.00} ({expense.Category}).");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
