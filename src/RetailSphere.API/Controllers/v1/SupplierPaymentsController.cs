using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.SupplierPayments.GetSupplierPaymentById;
using RetailSphere.Application.Features.SupplierPayments.GetSupplierPayments;
using RetailSphere.Application.Features.SupplierPayments.RecordSupplierPayment;
using RetailSphere.Application.Features.SupplierPayments.ReverseSupplierPayment;
using RetailSphere.Application.Features.SupplierPayments.UpdateSupplierPayment;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/supplier-payments")]
[Authorize]
public sealed class SupplierPaymentsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "purchasing.payments.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] long? supplierId,
        [FromQuery] long? purchaseInvoiceId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetSupplierPaymentsQuery(effectivePage, effectivePageSize, supplierId, purchaseInvoiceId, fromDate, toDate),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "purchasing.payments.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetSupplierPaymentByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "purchasing.payments.record")]
    public async Task<IActionResult> Record(RecordSupplierPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new RecordSupplierPaymentCommand(
                request.SupplierId, request.PurchaseInvoiceId, request.BranchId, request.PaymentDate, request.Amount,
                request.PaymentMethod, request.ReferenceNumber, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "purchasing.payments.edit")]
    public async Task<IActionResult> Update(long id, UpdateSupplierPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateSupplierPaymentCommand(id, request.PaymentDate, request.Amount, request.PaymentMethod, request.ReferenceNumber, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/reverse")]
    [Authorize(Policy = "purchasing.payments.edit")]
    public async Task<IActionResult> Reverse(long id, ReverseSupplierPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ReverseSupplierPaymentCommand(id, request.Reason), cancellationToken);
        return HandleResult(result);
    }
}
