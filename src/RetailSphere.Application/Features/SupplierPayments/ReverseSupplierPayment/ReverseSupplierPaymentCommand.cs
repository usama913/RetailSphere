using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.SupplierPayments.ReverseSupplierPayment;

public sealed record ReverseSupplierPaymentCommand(long Id, string Reason) : IRequest<Result<SupplierPaymentDto>>;
