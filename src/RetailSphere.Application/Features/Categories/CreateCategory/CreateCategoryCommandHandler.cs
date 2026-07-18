using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Contracts.Catalog;
using RetailSphere.Domain.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.CreateCategory;

public sealed class CreateCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService)
    : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryResult = Category.Create(request.Name, request.ParentCategoryId);
        if (categoryResult.IsFailure)
            return Result.Failure<CategoryDto>(categoryResult.Error);

        var category = categoryResult.Value;
        categoryRepository.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        auditLogService.Log("Category", category.Id.ToString(), "Created", $"Created category '{category.Name}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(CategoryMappings.ToDto(category));
    }
}
