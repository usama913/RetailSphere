using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(long Id, string Name, long? ParentCategoryId) : IRequest<Result<CategoryDto>>;
