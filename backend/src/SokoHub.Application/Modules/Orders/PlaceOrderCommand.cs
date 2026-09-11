using MediatR;
using SokoHub.Contracts.Orders;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Common.Specifications;

namespace SokoHub.Application.Modules.Orders;

public record PlaceOrderCommand(
    Guid CustomerId,
    Address ShippingAddress,
    IReadOnlyList<OrderLineDraft> Lines,
    Money ShippingTotal,
    Money DiscountTotal,
    Percentage TaxRate) : IRequest<Result<OrderResponse>>;

public sealed class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Result<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public PlaceOrderHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<OrderResponse>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CustomerId != _currentUser.Id)
        {
            return Result<OrderResponse>.Failure(new ApplicationError("unauthorized", "You can only place orders for yourself."));
        }

        // 1. Reserve Stock
        var reservations = new List<Guid>();
        foreach (var line in request.Lines)
        {
            var inventoryItem = await _unitOfWork.Repository<InventoryItem>().SingleAsync(
                new InventoryItemByVariantSpecification(line.VariantId), cancellationToken);

            if (inventoryItem == null)
            {
                return Result<OrderResponse>.Failure(new ApplicationError("stock_unavailable", $"Product {line.ProductName} is not available in stock."));
            }

            try
            {
                var res = inventoryItem.Reserve(_currentUser.Id, line.Quantity, DateTimeOffset.UtcNow.AddMinutes(30));
                reservations.Add(res.Id);
            }
            catch (Exception ex)
            {
                return Result<OrderResponse>.Failure(new ApplicationError("stock_reservation_failed", ex.Message));
            }
        }

        // 2. Create Order
        var orderNumber = OrderNumber.Generate();
        var order = Order.Place(
            request.CustomerId,
            orderNumber,
            request.ShippingAddress,
            request.Lines,
            request.ShippingTotal,
            request.DiscountTotal,
            request.TaxRate);

        await _unitOfWork.Repository<Order>().AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<OrderResponse>.Success(new OrderResponse(
            order.Id,
            order.Number.Value,
            order.CustomerId,
            order.Status.ToString(),
            order.GrandTotal.Value,
            order.Currency,
            DateTimeOffset.UtcNow));
    }
}

public class InventoryItemByVariantSpecification : Specification<InventoryItem>
{
    public InventoryItemByVariantSpecification(Guid variantId)
        : base(i => i.VariantId == variantId)
    {
    }
}
