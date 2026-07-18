using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.CreateCategory;

public sealed record CreateCategoryCommand(string Name, long? ParentCategoryId) : IRequest<Result<CategoryDto>>;
