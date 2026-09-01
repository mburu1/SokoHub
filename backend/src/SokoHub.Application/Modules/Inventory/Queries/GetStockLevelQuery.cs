using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Inventory;

namespace SokoHub.Application.Modules.Inventory.Queries;

public record GetStockLevelQuery(Guid InventoryItemId) : IRequest<StockLevelResponse>;

public record StockLevelResponse(
    Guid InventoryItemId,
    string Sku,
    int OnHand,
    int Reserved,
    int Available);

public sealed class GetStockLevelHandler : IRequestHandler<GetStockLevelQuery, StockLevelResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetStockLevelHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<StockLevelResponse> Handle(GetStockLevelQuery request, CancellationToken cancellationToken)
    {
        var item = await _unitOfWork.Repository<InventoryItem>().GetByIdAsync(request.InventoryItemId, cancellationToken);

        if (item == null)
        {
            throw new KeyNotFoundException("Inventory item not found.");
        }

        return new StockLevelResponse(
            item.Id,
            item.Sku.Value,
            item.OnHand,
            item.Reserved,
            item.Available);
    }
}
