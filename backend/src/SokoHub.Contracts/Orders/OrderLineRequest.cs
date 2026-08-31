namespace SokoHub.Contracts.Orders;

public record OrderLineRequest(
    Guid ProductId,
    Guid VariantId,
    string Sku,
    decimal UnitPrice,
    int Quantity);
