using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Inventory;

public record AdjustStockCommand(
    Guid InventoryItemId,
    int Delta,
    AdjustmentReason Reason,
    string Note) : IRequest<bool>;

public sealed class AdjustStockHandler : IRequestHandler<AdjustStockCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AdjustStockHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().GetByIdAsync(request.InventoryItemId, cancellationToken);

        if (item == null)
        {
            throw new KeyNotFoundException("Inventory item not found.");
        }

        item.Adjust(request.Delta, request.Reason, request.Note);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
