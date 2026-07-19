using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.PurchaseInvoices.CreatePurchaseInvoice;
using RetailSphere.Application.Features.PurchaseInvoices.DeletePurchaseInvoice;
using RetailSphere.Application.Features.PurchaseInvoices.GetPurchaseInvoiceById;
using RetailSphere.Application.Features.PurchaseInvoices.GetPurchaseInvoices;
using RetailSphere.Application.Features.PurchaseInvoices.UpdatePurchaseInvoice;
using RetailSphere.Contracts.Purchasing;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/purchase-invoices")]
[Authorize]
public sealed class PurchaseInvoicesController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "purchasing.invoices.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] long? supplierId,
        [FromQuery] long? branchId,
        [FromQuery] string? paymentStatus,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetPurchaseInvoicesQuery(effectivePage, effectivePageSize, supplierId, branchId, paymentStatus, fromDate, toDate),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "purchasing.invoices.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetPurchaseInvoiceByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "purchasing.invoices.create")]
    public async Task<IActionResult> Create(CreatePurchaseInvoiceRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreatePurchaseInvoiceCommand(
                request.SupplierId, request.BranchId, request.PurchaseOrderId, request.SupplierInvoiceNumber,
                request.InvoiceDate, request.DueDate, request.PaymentTerms, request.SubtotalAmount,
                request.DiscountAmount, request.TaxAmount, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "purchasing.invoices.edit")]
    public async Task<IActionResult> Update(long id, UpdatePurchaseInvoiceRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdatePurchaseInvoiceCommand(
                id, request.SupplierInvoiceNumber, request.InvoiceDate, request.DueDate, request.PaymentTerms,
                request.SubtotalAmount, request.DiscountAmount, request.TaxAmount, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "purchasing.invoices.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeletePurchaseInvoiceCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
