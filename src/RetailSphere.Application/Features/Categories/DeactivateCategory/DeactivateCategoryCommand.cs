using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.DeactivateCategory;

public sealed record DeactivateCategoryCommand(long Id) : IRequest<Result>;
