using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.SupplierLedger.GetSupplierAgingReport;
using RetailSphere.Application.Features.SupplierLedger.GetSupplierLedger;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/suppliers")]
[Authorize]
public sealed class SupplierLedgerController(ISender sender) : ApiControllerBase
{
    [HttpGet("{id:long}/ledger")]
    [Authorize(Policy = "purchasing.ledger.view")]
    public async Task<IActionResult> GetLedger(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSupplierLedgerQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("aging-report")]
    [Authorize(Policy = "purchasing.ledger.view")]
    public async Task<IActionResult> GetAgingReport([FromQuery] long? supplierId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSupplierAgingReportQuery(supplierId), cancellationToken);
        return HandleResult(result);
    }
}
