using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.GetSupplierPaymentById;

public sealed record GetSupplierPaymentByIdQuery(long Id) : IRequest<Result<SupplierPaymentDto>>;
