using MediatR;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Common;
using SokoHub.Contracts.Events.Orders;
using SokoHub.Contracts.Orders;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;

namespace SokoHub.Application.Orders;

public record PlaceOrderCommand(
    Guid CustomerId,
    IReadOnlyList<OrderLineRequest> Items,
    AddressDto ShippingAddress,
    decimal ShippingTotal,
    decimal DiscountTotal,
    string? CouponCode = null,
    string? CustomerNotes = null) : IRequest<Result<OrderResponse>>;

public sealed class PlaceOrderHandler : IRequestHandler<PlaceOrderCommand, Result<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;

    public PlaceOrderHandler(IUnitOfWork unitOfWork, IEventBus eventBus)
    {
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public async Task<Result<OrderResponse>> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var orderNumber = OrderNumber.Next();
        var address = Address.Create(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.County,
            request.ShippingAddress.PostalCode,
            request.ShippingAddress.Country);

        var lineDrafts = request.Items.Select(i => new OrderLineDraft(
            Guid.NewGuid(),
            i.ProductId,
            i.VariantId,
            Sku.From(i.Sku),
            i.Sku,
            Money.Create(i.UnitPrice, "KES"),
            i.Quantity)).ToList();

        var order = Order.Place(
            request.CustomerId,
            orderNumber,
            address,
            lineDrafts,
            Money.Create(request.ShippingTotal, "KES"),
            Money.Create(request.DiscountTotal, "KES"),
            Percentage.Create(0.16m)); // Kenyan VAT 16%

        await _unitOfWork.Repository<Order>().AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish OrderCreated event
        var orderItemsDto = order.Items.Select(item => new OrderItemDto(
            item.ProductId,
            item.VariantId,
            item.ProductName,
            item.Sku,
            item.UnitPrice.Amount,
            item.Quantity,
            item.VendorId)).ToList();

        await _eventBus.PublishAsync(new OrderCreatedIntegrationEvent(
            order.Id,
            order.Number.Value,
            order.CustomerId,
            order.GrandTotal.Amount,
            order.Currency,
            orderItemsDto), cancellationToken);

        return Result<OrderResponse>.Success(new OrderResponse(
            order.Id,
            order.Number.Value,
            order.CustomerId,
            order.Status.ToString(),
            order.GrandTotal.Amount,
            order.Currency,
            order.CreatedAt));
    }
}
