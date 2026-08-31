namespace SokoHub.Contracts.Orders;

public record OrderPlaceRequest(
    Guid CustomerId,
    SokoHub.Contracts.Orders.OrderLineRequest[] Items,
    SokoHub.Domain.Common.ValueObjects.Address ShippingAddress,
    decimal ShippingTotal,
    decimal DiscountTotal);
