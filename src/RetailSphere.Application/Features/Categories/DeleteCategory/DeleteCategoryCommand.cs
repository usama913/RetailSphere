using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(long Id) : IRequest<Result>;
