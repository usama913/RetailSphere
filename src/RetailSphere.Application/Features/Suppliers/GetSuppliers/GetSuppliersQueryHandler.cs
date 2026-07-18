using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.GetSuppliers;

public sealed class GetSuppliersQueryHandler(ISupplierRepository supplierRepository)
    : IRequestHandler<GetSuppliersQuery, Result<IReadOnlyList<SupplierDto>>>
{
    public async Task<Result<IReadOnlyList<SupplierDto>>> Handle(GetSuppliersQuery request, CancellationToken cancellationToken)
    {
        var suppliers = await supplierRepository.GetAllAsync(request.IncludeInactive, cancellationToken);
        var dtos = suppliers.Select(SupplierMappings.ToDto).ToList();

        return Result.Success<IReadOnlyList<SupplierDto>>(dtos);
    }
}
