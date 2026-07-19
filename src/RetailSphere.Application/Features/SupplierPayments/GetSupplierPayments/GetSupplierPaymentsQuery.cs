using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.GetSupplierPayments;

public sealed record GetSupplierPaymentsQuery(
    int Page,
    int PageSize,
    long? SupplierId,
    long? PurchaseInvoiceId,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<Result<PagedResult<SupplierPaymentDto>>>;
