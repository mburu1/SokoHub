using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Inventory;

public record ReserveStockCommand(
    Guid InventoryItemId,
    Guid OwnerId,
    int Quantity,
    int ExpirationMinutes = 15) : IRequest<Guid>;

public sealed class ReserveStockHandler : IRequestHandler<ReserveStockCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReserveStockHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().GetByIdAsync(request.InventoryItemId, cancellationToken);

        if (item == null)
        {
            throw new KeyNotFoundException("Inventory item not found.");
        }

        var reservation = item.Reserve(
            request.OwnerId,
            request.Quantity,
            DateTimeOffset.UtcNow.AddMinutes(request.ExpirationMinutes));

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return reservation.Id;
    }
}
