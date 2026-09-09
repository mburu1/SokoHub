using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Inventory;

public record AdjustStockCommand(
    Guid InventoryItemId,
    int Delta,
    AdjustmentReason Reason,
    string Note) : IRequest<Result>;

public sealed class AdjustStockHandler : IRequestHandler<AdjustStockCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AdjustStockHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AdjustStockCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().GetByIdAsync(request.InventoryItemId, cancellationToken);

        if (item == null)
        {
            return Result.Failure(new ApplicationError("inventory_item_not_found", "Inventory item not found."));
        }

        try
        {
            item.Adjust(request.Delta, request.Reason, request.Note);
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("adjustment_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
