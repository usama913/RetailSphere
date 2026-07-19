using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Finance.Expenses.GetExpenseCategories;

public sealed record GetExpenseCategoriesQuery : IRequest<Result<IReadOnlyList<string>>>;
