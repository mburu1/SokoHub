namespace SokoHub.Domain.Modules.Checkout;

public sealed class CheckoutItem : Entity
{
    private CheckoutItem()
    {
    }

    private CheckoutItem(
        Guid id,
        Guid checkoutSessionId,
        Guid vendorId,
        Guid productId,
        Guid variantId,
        Sku sku,
        string name,
        Money unitPrice,
        int quantity)
        : base(id)
    {
        CheckoutSessionId = checkoutSessionId;
        VendorId = vendorId;
        ProductId = productId;
        VariantId = variantId;
        Sku = sku;
        Name = name;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid CheckoutSessionId { get; private set; }

    public Guid VendorId { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid VariantId { get; private set; }

    public Sku Sku { get; private set; } = null!;

    public string Name { get; private set; } = string.Empty;

    public Money UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public Money LineTotal => UnitPrice * Quantity;

    internal static CheckoutItem Create(Guid checkoutSessionId, CheckoutItemDraft draft) =>
        new(
            Guid.Empty,
            checkoutSessionId,
            Ensure.NotEmpty(draft.VendorId),
            Ensure.NotEmpty(draft.ProductId),
            Ensure.NotEmpty(draft.VariantId),
            draft.Sku,
            Ensure.MaxLength(Ensure.NotBlank(draft.Name), 200),
            draft.UnitPrice,
            Ensure.Positive(draft.Quantity));
}
