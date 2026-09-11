using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Orders;

public record OrderCreatedIntegrationEvent(
    Guid OrderId,
    string OrderNumber,
    Guid CustomerId,
    decimal TotalAmount,
    string Currency,
    IReadOnlyList<OrderItemDto> Items) : IntegrationEvent;

public record OrderItemDto(
    Guid ProductId,
    Guid VariantId,
    string ProductName,
    string Sku,
    decimal UnitPrice,
    int Quantity,
    Guid VendorId);
