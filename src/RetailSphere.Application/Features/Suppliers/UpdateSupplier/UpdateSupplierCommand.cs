using MediatR;
using RetailSphere.Contracts.Purchasing;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Suppliers.UpdateSupplier;

public sealed record UpdateSupplierCommand(
    long Id,
    string Name,
    string? ContactPerson,
    string? Email,
    string? Phone,
    string? Address,
    string? TaxNumber) : IRequest<Result<SupplierDto>>;
