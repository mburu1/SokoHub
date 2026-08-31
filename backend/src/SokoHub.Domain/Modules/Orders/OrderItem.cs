namespace SokoHub.Domain.Modules.Orders;

public sealed class OrderItem : Entity
{
    private OrderItem()
    {
    }

    private OrderItem(
        Guid id,
        Guid orderId,
        Guid vendorId,
        Guid productId,
        Guid variantId,
        Sku sku,
        string productName,
        Money unitPrice,
        int quantity)
        : base(id)
    {
        OrderId = orderId;
        VendorId = vendorId;
        ProductId = productId;
        VariantId = variantId;
        Sku = sku;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        LineTotal = unitPrice * quantity;
    }

    public Guid OrderId { get; private set; }

    public Guid VendorId { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid VariantId { get; private set; }

    public Sku Sku { get; private set; } = null!;

    public string ProductName { get; private set; } = string.Empty;

    public Money UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public Money LineTotal { get; private set; }

    internal static OrderItem Create(Guid orderId, OrderLineDraft draft) =>
        new(
            Guid.Empty,
            orderId,
            Ensure.NotEmpty(draft.VendorId),
            Ensure.NotEmpty(draft.ProductId),
            Ensure.NotEmpty(draft.VariantId),
            draft.Sku,
            Ensure.MaxLength(Ensure.NotBlank(draft.ProductName), 200),
            draft.UnitPrice,
            Ensure.Positive(draft.Quantity));
}
