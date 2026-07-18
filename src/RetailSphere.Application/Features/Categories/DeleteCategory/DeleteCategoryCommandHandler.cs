using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Failure(Error.NotFound("Category.NotFound", "Category not found."));

        if (await categoryRepository.HasChildrenAsync(request.Id, cancellationToken))
            return Result.Failure(Error.Conflict("Category.HasChildren", "This category has subcategories — move or delete them first."));

        categoryRepository.Remove(category);
        auditLogService.Log("Category", category.Id.ToString(), "Deleted", $"Deleted category '{category.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
