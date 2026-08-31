namespace SokoHub.Contracts.Orders;

public record OrderResponse(
    Guid Id,
    string OrderNumber,
    string Status,
    decimal GrandTotal,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OrderItemResponse> Items);

public record OrderItemResponse(
    Guid Id,
    string ProductName,
    string Sku,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);
