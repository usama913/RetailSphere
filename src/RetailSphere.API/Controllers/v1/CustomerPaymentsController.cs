using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.CustomerPayments.AllocateCustomerPayment;
using RetailSphere.Application.Features.CustomerPayments.GetCustomerPaymentById;
using RetailSphere.Application.Features.CustomerPayments.GetCustomerPayments;
using RetailSphere.Application.Features.CustomerPayments.RecordCustomerPayment;
using RetailSphere.Application.Features.CustomerPayments.RemoveCustomerPaymentAllocation;
using RetailSphere.Application.Features.CustomerPayments.ReverseCustomerPayment;
using RetailSphere.Application.Features.CustomerPayments.UpdateCustomerPayment;
using RetailSphere.Contracts.Customers;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customer-payments")]
[Authorize]
public sealed class CustomerPaymentsController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "customers.payments.view")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page,
        [FromQuery] int pageSize,
        [FromQuery] long? customerId,
        [FromQuery] long? branchId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken cancellationToken)
    {
        var effectivePage = page <= 0 ? 1 : page;
        var effectivePageSize = pageSize <= 0 ? 25 : pageSize;

        var result = await sender.Send(
            new GetCustomerPaymentsQuery(effectivePage, effectivePageSize, customerId, branchId, fromDate, toDate),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:long}")]
    [Authorize(Policy = "customers.payments.view")]
    public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomerPaymentByIdQuery(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "customers.payments.record")]
    public async Task<IActionResult> Record(RecordCustomerPaymentRequest request, CancellationToken cancellationToken)
    {
        var allocations = request.Allocations
            .Select(a => new RecordCustomerPaymentAllocationInput(a.SalesOrderId, a.Amount))
            .ToList();

        var result = await sender.Send(
            new RecordCustomerPaymentCommand(
                request.CustomerId, request.BranchId, request.PaymentDate, request.Amount,
                request.PaymentMethod, request.ReferenceNumber, request.Notes, allocations),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "customers.payments.edit")]
    public async Task<IActionResult> Update(long id, UpdateCustomerPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateCustomerPaymentCommand(id, request.PaymentDate, request.Amount, request.PaymentMethod, request.ReferenceNumber, request.Notes),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/reverse")]
    [Authorize(Policy = "customers.payments.edit")]
    public async Task<IActionResult> Reverse(long id, ReverseCustomerPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ReverseCustomerPaymentCommand(id, request.Reason), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/allocations")]
    [Authorize(Policy = "customers.payments.edit")]
    public async Task<IActionResult> Allocate(long id, AllocateCustomerPaymentRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new AllocateCustomerPaymentCommand(id, request.SalesOrderId, request.Amount), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}/allocations/{salesOrderId:long}")]
    [Authorize(Policy = "customers.payments.edit")]
    public async Task<IActionResult> RemoveAllocation(long id, long salesOrderId, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new RemoveCustomerPaymentAllocationCommand(id, salesOrderId), cancellationToken);
        return HandleResult(result);
    }
}
