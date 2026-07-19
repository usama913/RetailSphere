using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.CreateSupplier;

public sealed record CreateSupplierCommand(
    string Name,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address,
    string? TaxNumber,
    decimal? CreditLimit,
    string? PaymentTerms) : IRequest<Result<SupplierDto>>;
