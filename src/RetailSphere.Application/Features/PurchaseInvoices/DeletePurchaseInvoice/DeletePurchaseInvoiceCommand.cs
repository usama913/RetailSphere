using MediatR;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.PurchaseInvoices.DeletePurchaseInvoice;

public sealed record DeletePurchaseInvoiceCommand(long Id) : IRequest<Result>;
