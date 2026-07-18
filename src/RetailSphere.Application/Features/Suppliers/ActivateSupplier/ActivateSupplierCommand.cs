using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.ActivateSupplier;

public sealed record ActivateSupplierCommand(long Id) : IRequest<Result>;
