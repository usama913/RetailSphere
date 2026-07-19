using MediatR;
using RetailSphere.Application.Features.SupplierPayments.Common;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.Domain.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.GetSupplierPayments;

public sealed class GetSupplierPaymentsQueryHandler(ISupplierPaymentRepository supplierPaymentRepository, SupplierPaymentDtoAssembler supplierPaymentDtoAssembler)
    : IRequestHandler<GetSupplierPaymentsQuery, Result<PagedResult<SupplierPaymentDto>>>
{
    public async Task<Result<PagedResult<SupplierPaymentDto>>> Handle(GetSupplierPaymentsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await supplierPaymentRepository.SearchAsync(
            request.Page, request.PageSize, request.SupplierId, request.PurchaseInvoiceId, request.FromDate, request.ToDate, cancellationToken);

        var dtos = await supplierPaymentDtoAssembler.ToDtosAsync(items, cancellationToken);

        return Result.Success(new PagedResult<SupplierPaymentDto>
        {
            Data = dtos,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,
        });
    }
}
