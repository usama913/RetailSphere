using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.SalesOrders.CancelSalesOrder;
using RetailSphere.Application.Features.SalesOrders.CreateSalesOrder;
using RetailSphere.Application.Features.SalesOrders.GetSalesOrderById;
using RetailSphere.Application.Features.SalesOrders.GetSalesOrders;
using RetailSphere.Contracts.Sales;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales-orders")]
[Authorize]
public sealed class SalesOrdersController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "sales.orders.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] string? search,
        [FromQuery] long? branchId,
        [FromQuery] long? customerId,
        [FromQuery] string? status,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetSalesOrdersQuery(effectivePage, effectivePageSize, search, branchId, customerId, status, fromDate, toDate),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "sales.orders.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSalesOrderByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "sales.pos.create")]
    public async Task<IActionResult> Create(CreateSalesOrderRequest request, CancellationToken cancellationToken)
    {
        var lines = request.Lines
            .Select(l => new CreateSalesOrderLineItem(l.ProductId, l.ProductVariantId, l.Quantity, l.DiscountAmount))
            .ToList();

        var result = await sender.Send(
            new CreateSalesOrderCommand(
                request.BranchId, request.CustomerId, request.PaymentMethod, request.OrderDiscountAmount, request.AmountPaid, request.Notes,
                request.PaymentTerms, request.DueDate, request.OverrideCreditLimit, lines),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/cancel")]
    [Authorize(Policy = "sales.pos.create")]
    public async Task<IActionResult> Cancel(long id, CancelSalesOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CancelSalesOrderCommand(id, request.Reason), cancellationToken);
        return HandleResult(result);
    }
}
