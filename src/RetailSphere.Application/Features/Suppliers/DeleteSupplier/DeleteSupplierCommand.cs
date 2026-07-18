using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.DeleteSupplier;

public sealed record DeleteSupplierCommand(long Id) : IRequest<Result>;
