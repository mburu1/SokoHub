using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Inventory;

public record CreateInventoryItemCommand(
    Guid WarehouseId,
    Guid VariantId,
    string Sku,
    int InitialQuantity) : IRequest<Guid>;

public sealed class CreateInventoryItemHandler : IRequestHandler<CreateInventoryItemCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateInventoryItemHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        var variant = await _unitOfWork.Repository<ProductVariant>().GetByIdAsync(request.VariantId, cancellationToken);
        if (variant == null)
        {
            throw new KeyNotFoundException("Product variant not found.");
        }

        var warehouse = await _unitOfWork.Repository<Warehouse>().GetByIdAsync(request.WarehouseId, cancellationToken);
        if (warehouse == null)
        {
            throw new KeyNotFoundException("Warehouse not found.");
        }

        var item = InventoryItem.Open(
            request.WarehouseId,
            request.VariantId,
            variant.Sku,
            request.InitialQuantity);

        await _unitOfWork.Repository<InventoryItem>().AddAsync(item, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return item.Id;
    }
}
