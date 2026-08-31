namespace SokoHub.Domain.Modules.Cart;

public sealed class CartItem : Entity
{
    private CartItem()
    {
    }

    private CartItem(
        Guid id,
        Guid cartId,
        Guid vendorId,
        Guid productId,
        Guid variantId,
        Sku sku,
        string name,
        Money unitPrice,
        int quantity)
        : base(id)
    {
        CartId = cartId;
        VendorId = vendorId;
        ProductId = productId;
        VariantId = variantId;
        Sku = sku;
        Name = name;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid CartId { get; private set; }

    public Guid VendorId { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid VariantId { get; private set; }

    public Sku Sku { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;

    public Money UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public Money LineTotal => UnitPrice * Quantity;

    internal static CartItem Create(
        Guid cartId,
        Guid vendorId,
        Guid productId,
        Guid variantId,
        Sku sku,
        string name,
        Money unitPrice,
        int quantity) =>
        new(
            Guid.Empty,
            cartId,
            Ensure.NotEmpty(vendorId),
            Ensure.NotEmpty(productId),
            Ensure.NotEmpty(variantId),
            sku,
            Ensure.MaxLength(Ensure.NotBlank(name), 200),
            unitPrice,
            Ensure.Positive(quantity));

    internal void Increase(int quantity)
    {
        Quantity += Ensure.Positive(quantity);
        Touch();
    }

    internal void SetQuantity(int quantity)
    {
        Quantity = Ensure.Positive(quantity);
        Touch();
    }
}
