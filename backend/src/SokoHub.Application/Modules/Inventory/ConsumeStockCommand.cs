using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Inventory;

public record ConsumeStockCommand(
    Guid InventoryItemId,
    Guid ReservationId) : IRequest<bool>;

public sealed class ConsumeStockHandler : IRequestHandler<ConsumeStockCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConsumeStockHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ConsumeStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().GetByIdAsync(request.InventoryItemId, cancellationToken);

        if (item == null)
        {
            throw new KeyNotFoundException("Inventory item not found.");
        }

        item.Consume(request.ReservationId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
