using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Finance.Expenses.CreateExpense;
using RetailSphere.Application.Features.Finance.Expenses.DeleteExpense;
using RetailSphere.Application.Features.Finance.Expenses.GetExpenseCategories;
using RetailSphere.Application.Features.Finance.Expenses.GetExpenses;
using RetailSphere.Application.Features.Finance.Expenses.UpdateExpense;
using RetailSphere.Contracts.Finance;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/expenses")]
[Authorize]
public sealed class ExpensesController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "finance.expenses.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] long? branchId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetExpensesQuery(effectivePage, effectivePageSize, branchId, fromDate, toDate, category),
            cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// The merged suggested-defaults + already-in-use category list for the "Add
    /// Expense" dropdowns (see GetExpenseCategoriesQueryHandler). Left at the base
    /// [Authorize] (any authenticated user) rather than a finance-specific policy —
    /// it's just a list of category name strings, not sensitive financial data, and
    /// both expense viewers and the POS quick-add-expense flow need to read it.
    /// </summary>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetExpenseCategoriesQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "finance.expenses.edit")]
    public async Task<IActionResult> Create(CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateExpenseCommand(request.BranchId, request.ExpenseDate, request.Amount, request.Category, request.Description, request.PaidFromCash),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "finance.expenses.edit")]
    public async Task<IActionResult> Update(long id, UpdateExpenseRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateExpenseCommand(id, request.ExpenseDate, request.Amount, request.Category, request.Description, request.PaidFromCash),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "finance.expenses.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteExpenseCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
