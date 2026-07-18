using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.GetSuppliers;

public sealed record GetSuppliersQuery(bool IncludeInactive) : IRequest<Result<IReadOnlyList<SupplierDto>>>;
