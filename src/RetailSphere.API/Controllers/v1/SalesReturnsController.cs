using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.SalesReturns.CreateSalesReturn;
using RetailSphere.Application.Features.SalesReturns.GetSalesReturnById;
using RetailSphere.Application.Features.SalesReturns.GetSalesReturns;
using RetailSphere.Contracts.Sales;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/sales-returns")]
[Authorize]
public sealed class SalesReturnsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "sales.returns.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] long? branchId,
        [FromQuery] long? customerId,
        [FromQuery] long? salesOrderId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetSalesReturnsQuery(effectivePage, effectivePageSize, branchId, customerId, salesOrderId, fromDate, toDate),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "sales.returns.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSalesReturnByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "sales.returns.create")]
    public async Task<IActionResult> Create(CreateSalesReturnRequest request, CancellationToken cancellationToken)
    {
        var lines = request.Lines
            .Select(l => new CreateSalesReturnLineItem(l.SalesOrderLineId, l.Quantity))
            .ToList();

        var result = await sender.Send(new CreateSalesReturnCommand(request.SalesOrderId, request.Reason, lines), cancellationToken);
        return HandleResult(result);
    }
}
