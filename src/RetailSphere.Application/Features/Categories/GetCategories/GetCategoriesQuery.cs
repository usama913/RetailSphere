using MediatR;
using RetailSphere.Contracts.Catalog;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.GetCategories;

public sealed record GetCategoriesQuery(bool IncludeInactive) : IRequest<Result<IReadOnlyList<CategoryDto>>>;
