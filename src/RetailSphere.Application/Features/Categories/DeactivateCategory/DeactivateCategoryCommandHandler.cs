using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.DeactivateCategory;

public sealed class DeactivateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<DeactivateCategoryCommand, Result>
{
    public async Task<Result> Handle(DeactivateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Failure(Error.NotFound("Category.NotFound", "Category not found."));

        category.Deactivate();
        categoryRepository.Update(category);
        auditLogService.Log("Category", category.Id.ToString(), "Deactivated", $"Deactivated category '{category.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
