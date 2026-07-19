using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.DeleteExpense;

public sealed record DeleteExpenseCommand(long Id) : IRequest<Result>;
