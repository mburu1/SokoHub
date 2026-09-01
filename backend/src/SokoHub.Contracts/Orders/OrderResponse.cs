namespace SokoHub.Contracts.Orders;

public record OrderResponse(
    Guid Id,
    string OrderNumber,
    Guid CustomerId,
    string Status,
    decimal GrandTotal,
    string Currency,
    DateTimeOffset CreatedAt);
