using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.DeactivateSupplier;

public sealed record DeactivateSupplierCommand(long Id) : IRequest<Result>;
