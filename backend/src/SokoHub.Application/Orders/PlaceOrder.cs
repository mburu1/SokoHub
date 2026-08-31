using MediatR;
using SokoHub.Contracts.Orders;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Orders;

namespace SokoHub.Application.Orders;

public record PlaceOrderCommand(
    Guid CustomerId,
    IReadOnlyList<OrderLineRequest> Items,
    SokoHub.Domain.Common.ValueObjects.Address ShippingAddress,
    decimal ShippingTotal,
    decimal DiscountTotal) : IRequest<OrderResponse>;

public sealed class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public PlaceOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var orderNumber = OrderNumber.Next();
        var lineDrafts = request.Items.Select(i => new OrderLineDraft(
            Guid.Empty, // VendorId would be looked up from Product Variant
            Guid.Empty, // ProductId
            Guid.Empty, // VariantId
            i.Sku,
            i.ProductName,
            Money.Create(i.UnitPrice),
            i.Quantity)).ToList();

        var order = Order.Place(
            request.CustomerId,
            orderNumber,
            request.ShippingAddress,
            lineDrafts,
            Money.Create(request.ShippingTotal),
            Money.Create(request.DiscountTotal),
            Percentage.Create(0.16m)); // Default VAT

        // await _unitOfWork.Repository<Order>().AddAsync(order);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new OrderResponse(
            order.Id,
            order.Number.Value,
            order.Status.ToString(),
            order.GrandTotal.Amount,
            DateTimeOffset.UtcNow,
            []);
    }
}
