using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Finance.Expenses.Common;
using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.UpdateExpense;

public sealed class UpdateExpenseCommandHandler(
    IExpenseRepository expenseRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    ExpenseDtoAssembler expenseDtoAssembler)
    : IRequestHandler<UpdateExpenseCommand, Result<ExpenseDto>>
{
    public async Task<Result<ExpenseDto>> Handle(UpdateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = await expenseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (expense is null)
            return Result.Failure<ExpenseDto>(Error.NotFound("Expense.NotFound", "Expense not found."));

        var updateResult = expense.UpdateDetails(request.ExpenseDate, request.Amount, request.Category, request.Description, request.PaidFromCash);
        if (updateResult.IsFailure)
            return Result.Failure<ExpenseDto>(updateResult.Error);

        expenseRepository.Update(expense);
        auditLogService.Log("Expense", expense.Id.ToString(), "Updated", $"Updated expense of {expense.Amount:0.00} ({expense.Category}).");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await expenseDtoAssembler.ToDtoAsync(expense, cancellationToken);
        return Result.Success(dto);
    }
}
