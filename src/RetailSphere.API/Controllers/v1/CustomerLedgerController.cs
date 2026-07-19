using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.CustomerLedger.GetCustomerAgingReport;
using RetailSphere.Application.Features.CustomerLedger.GetCustomerCreditSummary;
using RetailSphere.Application.Features.CustomerLedger.GetCustomerLedger;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customers")]
[Authorize]
public sealed class CustomerLedgerController(ISender sender) : ApiControllerBase
{
    [HttpGet("{id:long}/ledger")]
    [Authorize(Policy = "customers.ledger.view")]
    public async Task<IActionResult> GetLedger(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomerLedgerQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("aging-report")]
    [Authorize(Policy = "customers.ledger.view")]
    public async Task<IActionResult> GetAgingReport(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomerAgingReportQuery(), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}/credit-summary")]
    [Authorize(Policy = "customers.view")]
    public async Task<IActionResult> GetCreditSummary(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomerCreditSummaryQuery(id), cancellationToken);
        return HandleResult(result);
    }
}
