using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.GetPurchaseInvoiceById;

public sealed record GetPurchaseInvoiceByIdQuery(long Id) : IRequest<Result<PurchaseInvoiceDto>>;
