using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Inventory;

public record ConsumeStockCommand(
    Guid InventoryItemId,
    Guid ReservationId) : IRequest<Result>;

public sealed class ConsumeStockHandler : IRequestHandler<ConsumeStockCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConsumeStockHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConsumeStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().GetByIdAsync(request.InventoryItemId, cancellationToken);

        if (item == null)
        {
            return Result.Failure(new ApplicationError("inventory_item_not_found", "Inventory item not found."));
        }

        try
        {
            item.Consume(request.ReservationId);
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("consumption_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
