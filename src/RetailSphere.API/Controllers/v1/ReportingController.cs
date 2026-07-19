using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Reporting.GetFinancialSummary;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/reporting")]
[Authorize]
public sealed class ReportingController(ISender sender) : ApiControllerBase
{
    [HttpGet("financial-summary")]
    [Authorize(Policy = "reporting.view")]
    public async Task<IActionResult> GetFinancialSummary(CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetFinancialSummaryQuery(), cancellationToken);
        return HandleResult(result);
    }
}
