using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.UpdateSupplierCreditTerms;

public sealed record UpdateSupplierCreditTermsCommand(long Id, decimal? CreditLimit, string? PaymentTerms) : IRequest<Result<SupplierDto>>;
