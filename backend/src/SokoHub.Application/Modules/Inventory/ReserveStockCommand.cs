using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Inventory;

public record ReserveStockCommand(
    Guid InventoryItemId,
    Guid OwnerId,
    int Quantity,
    int ExpirationMinutes = 15) : IRequest<Result<Guid>>;

public sealed class ReserveStockHandler : IRequestHandler<ReserveStockCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReserveStockHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ReserveStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().GetByIdAsync(request.InventoryItemId, cancellationToken);

        if (item == null)
        {
            return Result<Guid>.Failure(new ApplicationError("inventory_item_not_found", "Inventory item not found."));
        }

        try
        {
            var reservation = item.Reserve(
                request.OwnerId,
                request.Quantity,
                DateTimeOffset.UtcNow.AddMinutes(request.ExpirationMinutes));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(reservation.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(new ApplicationError("reservation_failed", ex.Message));
        }
    }
}
