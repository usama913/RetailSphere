using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.ActivateCategory;

public sealed class ActivateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<ActivateCategoryCommand, Result>
{
    public async Task<Result> Handle(ActivateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Failure(Error.NotFound("Category.NotFound", "Category not found."));

        category.Activate();
        categoryRepository.Update(category);
        auditLogService.Log("Category", category.Id.ToString(), "Activated", $"Activated category '{category.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
