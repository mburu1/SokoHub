using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Inventory;

public record CreateInventoryItemCommand(
    Guid WarehouseId,
    Guid VariantId,
    string Sku,
    int InitialQuantity) : IRequest<Result<Guid>>;

public sealed class CreateInventoryItemHandler : IRequestHandler<CreateInventoryItemCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryItemHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var variant = await _unitOfWork.Repository<ProductVariant>().GetByIdAsync(request.VariantId, cancellationToken);
        if (variant == null)
        {
            return Result<Guid>.Failure(new ApplicationError("variant_not_found", "Product variant not found."));
        }

        var warehouse = await _unitOfWork.Repository<Warehouse>().GetByIdAsync(request.WarehouseId, cancellationToken);
        if (warehouse == null)
        {
            return Result<Guid>.Failure(new ApplicationError("warehouse_not_found", "Warehouse not found."));
        }

        var item = InventoryItem.Open(
            request.WarehouseId,
            request.VariantId,
            variant.Sku,
            request.InitialQuantity);

        await _unitOfWork.Repository<InventoryItem>().AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(item.Id);
    }
}
