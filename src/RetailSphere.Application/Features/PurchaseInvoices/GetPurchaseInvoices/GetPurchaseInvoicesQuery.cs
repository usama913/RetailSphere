using MediatR;
using RetailSphere.Contracts.Common;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.GetPurchaseInvoices;

public sealed record GetPurchaseInvoicesQuery(
    int Page,
    int PageSize,
    long? SupplierId,
    long? BranchId,
    string? PaymentStatus,
    DateTime? FromDate,
    DateTime? ToDate) : IRequest<Result<PagedResult<PurchaseInvoiceDto>>>;
