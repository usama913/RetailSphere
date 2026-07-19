using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Finance.Expenses.Common;
using RetailSphere.Common;
using RetailSphere.Contracts.Finance;
using RetailSphere.Domain.Finance;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.CreateExpense;

public sealed class CreateExpenseCommandHandler(
    IExpenseRepository expenseRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    ExpenseDtoAssembler expenseDtoAssembler,
    ICurrentUserService currentUserService)
    : IRequestHandler<CreateExpenseCommand, Result<ExpenseDto>>
{
    public async Task<Result<ExpenseDto>> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var expenseResult = Expense.Create(
            request.BranchId, request.ExpenseDate, request.Amount, request.Category, request.Description, request.PaidFromCash, currentUserService.UserId);
        if (expenseResult.IsFailure)
            return Result.Failure<ExpenseDto>(expenseResult.Error);

        var expense = expenseResult.Value;
        expenseRepository.Add(expense);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("Expense", expense.Id.ToString(), "Created", $"Recorded expense of {expense.Amount:0.00} ({expense.Category}).");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await expenseDtoAssembler.ToDtoAsync(expense, cancellationToken);
        return Result.Success(dto);
    }
}
