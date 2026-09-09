using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Inventory;

public record ReserveInventoryCommand(
    Guid VariantId,
    int Quantity) : IRequest<Result<Guid>>;

public sealed class ReserveInventoryHandler : IRequestHandler<ReserveInventoryCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public ReserveInventoryHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(ReserveInventoryCommand request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().SingleAsync(
            new InventoryItemByVariantSpecification(request.VariantId), cancellationToken);

        if (item == null)
        {
            return Result<Guid>.Failure(new ApplicationError("inventory_item_not_found", "Inventory item not found."));
        }

        try
        {
            var reservation = item.Reserve(_currentUser.Id, request.Quantity, DateTimeOffset.UtcNow.AddMinutes(15));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(reservation.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure(new ApplicationError("reservation_failed", ex.Message));
        }
    }
}

public class InventoryItemByVariantSpecification : Specification<InventoryItem>
{
    public InventoryItemByVariantSpecification(Guid variantId)
        : base(i => i.VariantId == variantId)
    {
    }
}
