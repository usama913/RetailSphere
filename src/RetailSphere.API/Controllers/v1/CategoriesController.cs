using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailSphere.API.Common;
using RetailSphere.Application.Features.Categories.ActivateCategory;
using RetailSphere.Application.Features.Categories.CreateCategory;
using RetailSphere.Application.Features.Categories.DeactivateCategory;
using RetailSphere.Application.Features.Categories.DeleteCategory;
using RetailSphere.Application.Features.Categories.GetCategories;
using RetailSphere.Application.Features.Categories.UpdateCategory;
using RetailSphere.Contracts.Catalog;

namespace RetailSphere.API.Controllers.v1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
[Authorize]
public sealed class CategoriesController(ISender sender) : ApiControllerBase
{
    [HttpGet]
    [Authorize(Policy = "catalog.categories.view")]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new GetCategoriesQuery(includeInactive), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost]
    [Authorize(Policy = "catalog.categories.edit")]
    public async Task<IActionResult> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new CreateCategoryCommand(request.Name, request.ParentCategoryId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "catalog.categories.edit")]
    public async Task<IActionResult> Update(long id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new UpdateCategoryCommand(id, request.Name, request.ParentCategoryId), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/activate")]
    [Authorize(Policy = "catalog.categories.edit")]
    public async Task<IActionResult> Activate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new ActivateCategoryCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:long}/deactivate")]
    [Authorize(Policy = "catalog.categories.edit")]
    public async Task<IActionResult> Deactivate(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeactivateCategoryCommand(id), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "catalog.categories.edit")]
    public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
    {
        var result = await sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
