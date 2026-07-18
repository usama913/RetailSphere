using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Customers.ActivateCustomer;
using RetailSphere.Application.Features.Customers.CreateCustomer;
using RetailSphere.Application.Features.Customers.DeactivateCustomer;
using RetailSphere.Application.Features.Customers.DeleteCustomer;
using RetailSphere.Application.Features.Customers.GetCustomers;
using RetailSphere.Application.Features.Customers.UpdateCustomer;
using RetailSphere.Contracts.Customers;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/customers")]
[Authorize]
public sealed class CustomersController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "customers.view")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCustomersQuery(includeInactive), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new CreateCustomerCommand(request.Name, request.Phone, request.Email, request.Address),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Update(long id, UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new UpdateCustomerCommand(id, request.Name, request.Phone, request.Email, request.Address),
            cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/activate")]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateCustomerCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/deactivate")]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateCustomerCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "customers.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteCustomerCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
