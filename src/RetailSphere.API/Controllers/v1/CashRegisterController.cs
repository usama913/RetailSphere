using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Finance.CashRegister.CloseCashRegisterSession;
using RetailSphere.Application.Features.Finance.CashRegister.GetCashRegisterSessions;
using RetailSphere.Application.Features.Finance.CashRegister.GetCurrentCashRegisterSession;
using RetailSphere.Application.Features.Finance.CashRegister.OpenCashRegisterSession;
using RetailSphere.Contracts.Finance;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cash-register")]
[Authorize]
public sealed class CashRegisterController(ISender sender) : ApiControllerBase
{
    [HttpGet("current")]
    [Authorize(Policy = "finance.cashregister.view")]
    public async Task<IActionResult> GetCurrent([FromQuery] long branchId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCurrentCashRegisterSessionQuery(branchId), cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Policy = "finance.cashregister.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] long? branchId,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(new GetCashRegisterSessionsQuery(effectivePage, effectivePageSize, branchId, status), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("open")]
    [Authorize(Policy = "finance.cashregister.operate")]
    public async Task<IActionResult> Open(OpenCashRegisterSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new OpenCashRegisterSessionCommand(request.BranchId, request.OpeningBalance, request.OpeningNotes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/close")]
    [Authorize(Policy = "finance.cashregister.operate")]
    public async Task<IActionResult> Close(long id, CloseCashRegisterSessionRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CloseCashRegisterSessionCommand(id, request.ClosingBalance, request.ClosingNotes),
            cancellationToken);
        return HandleResult(result);
    }
}
