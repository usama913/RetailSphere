using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.UpdateCategory;

public sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return Result.Failure<CategoryDto>(Error.NotFound("Category.NotFound", "Category not found."));

        var updateResult = category.UpdateDetails(request.Name, request.ParentCategoryId);
        if (updateResult.IsFailure)
            return Result.Failure<CategoryDto>(updateResult.Error);

        categoryRepository.Update(category);
        auditLogService.Log("Category", category.Id.ToString(), "Updated", $"Updated category '{category.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(CategoryMappings.ToDto(category));
    }
}
