using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierLedger.GetSupplierAgingReport;

public sealed record GetSupplierAgingReportQuery(long? SupplierId) : IRequest<Result<SupplierAgingReportDto>>;
