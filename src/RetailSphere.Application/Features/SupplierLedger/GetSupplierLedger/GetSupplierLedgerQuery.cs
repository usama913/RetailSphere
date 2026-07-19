using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierLedger.GetSupplierLedger;

public sealed record GetSupplierLedgerQuery(long SupplierId) : IRequest<Result<SupplierLedgerDto>>;
