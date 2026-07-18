using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Categories.ActivateCategory;

public sealed record ActivateCategoryCommand(long Id) : IRequest<Result>;
