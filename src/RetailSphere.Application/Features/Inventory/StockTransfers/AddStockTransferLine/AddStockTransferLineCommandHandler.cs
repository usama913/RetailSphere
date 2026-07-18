using MediatR;
using RetailSphere.Application.Common.Interfaces;
using RetailSphere.Application.Common.Services;
using RetailSphere.Application.Features.Inventory.StockTransfers.Common;
using RetailSphere.Contracts.Inventory;
using RetailSphere.Domain.Catalog;
using RetailSphere.Domain.Inventory;
using RetailSphere.SharedKernel;

namespace RetailSphere.Application.Features.Inventory.StockTransfers.AddStockTransferLine;

/// <summary>Resolves the requested SKU from Catalog and snapshots it onto the line — mirrors AddPurchaseOrderLineCommandHandler.</summary>
public sealed class AddStockTransferLineCommandHandler(
    IStockTransferRepository stockTransferRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    AuditLogService auditLogService,
    StockTransferDtoAssembler stockTransferDtoAssembler)
    : IRequestHandler<AddStockTransferLineCommand, Result<StockTransferDto>>
{
    public async Task<Result<StockTransferDto>> Handle(AddStockTransferLineCommand request, CancellationToken cancellationToken)
    {
        var stockTransfer = await stockTransferRepository.GetByIdAsync(request.StockTransferId, cancellationToken);
        if (stockTransfer is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("StockTransfer.NotFound", "Transfer not found."));

        var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("Product.NotFound", "Product not found."));

        var variant = product.Variants.FirstOrDefault(v => v.Id == request.ProductVariantId);
        if (variant is null)
            return Result.Failure<StockTransferDto>(Error.NotFound("Product.VariantNotFound", "Product variant not found."));

        var descriptionSnapshot = $"{product.Name} ({variant.Sku})";

        var addResult = stockTransfer.AddLine(product.Id, variant.Id, variant.Sku, descriptionSnapshot, request.QuantityRequested);
        if (addResult.IsFailure)
            return Result.Failure<StockTransferDto>(addResult.Error);

        stockTransferRepository.Update(stockTransfer);
        auditLogService.Log("StockTransfer", stockTransfer.Id.ToString(), "LineAdded", $"Added line '{variant.Sku}' to stock transfer '{stockTransfer.TransferNumber}'.");
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = await stockTransferDtoAssembler.ToDtoAsync(stockTransfer, cancellationToken);
        return Result.Success(dto);
    }
}
