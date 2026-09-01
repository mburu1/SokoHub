using MediatR;
using SokoHub.Contracts.Orders;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Domain.Modules.Inventory;

namespace SokoHub.Application.Modules.Orders;

public record PlaceOrderCommand(
    Guid CustomerId,
    Address ShippingAddress,
    IReadOnlyList<OrderLineDraft> Lines,
    Money ShippingTotal,
    Money DiscountTotal,
    Percentage TaxRate) : IRequest<OrderResponse>;

public sealed class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public PlaceOrderHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<OrderResponse> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        if (request.CustomerId != _currentUser.Id)
        {
            throw new UnauthorizedAccessException("You can only place orders for yourself.");
        }

        // 1. Reserve Stock
        var reservations = new List<Guid>();
        foreach (var line in request.Lines)
        {
            // We need to find the InventoryItem for this variant in the relevant warehouse.
            // For simplicity, assume a default warehouse.
            var inventoryItem = await _unitOfWork.Repository<InventoryItem>().SingleAsync(
                new InventoryItemByVariantSpecification(line.VariantId), cancellationToken);

            if (inventoryItem == null)
            {
                throw new InvalidOperationException($"Product {line.ProductName} is not available in stock.");
            }

            var res = inventoryItem.Reserve(_currentUser.Id.Value, line.Quantity, DateTimeOffset.UtcNow.AddMinutes(30));
            reservations.Add(res.Id);
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

        return new OrderResponse(
            order.Id,
            order.Number.Value,
            order.CustomerId,
            order.Status.ToString(),
            order.GrandTotal.Value,
            order.Currency,
            DateTimeOffset.UtcNow);
    }
}

public class InventoryItemByVariantSpecification : Specification<InventoryItem>
{
    public InventoryItemByVariantSpecification(Guid variantId)
        : base(i => i.VariantId == variantId)
    {
    }
}
