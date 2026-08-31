namespace SokoHub.Domain.Modules.Cart;

public sealed class Cart : AggregateRoot
{
    private readonly List<CartItem> _items = [];
    private readonly List<CartReservation> _reservations = [];

    private Cart()
    {
    }

    private Cart(Guid id, Guid customerId)
        : base(id)
    {
        CustomerId = customerId;
        Currency = Money.DefaultCurrency;
    }

    public Guid CustomerId { get; private set; }

    public string Currency { get; private set; } = Money.DefaultCurrency;

    public string? CouponCode { get; private set; }

    public IReadOnlyList<CartItem> Items => _items.AsReadOnly();

    public IReadOnlyList<CartReservation> Reservations => _reservations.AsReadOnly();

    public Money Subtotal => _items.Aggregate(Money.Zero(Currency), (sum, item) => sum + item.LineTotal);

    public static Cart Create(Guid customerId, Guid? id = null) =>
        new(id ?? Guid.Empty, Ensure.NotEmpty(customerId));

    public CartItem AddItem(Guid vendorId, Guid productId, Guid variantId, Sku sku, string name, Money unitPrice, int quantity)
    {
        Ensure.That(unitPrice.Currency == Currency, "currency_mismatch", "Item currency must match the cart.");
        var existing = _items.SingleOrDefault(i => i.VariantId == variantId);
        if (existing is not null)
        {
            existing.Increase(quantity);
            Touch();
            return existing;
        }

        var item = CartItem.Create(Id, vendorId, productId, variantId, sku, name, unitPrice, quantity);
        _items.Add(item);
        Touch();
        return item;
    }

    public void ChangeQuantity(Guid variantId, int quantity)
    {
        var item = RequireItem(variantId);
        if (quantity == 0)
        {
            _items.Remove(item);
        }
        else
        {
            item.SetQuantity(quantity);
        }

        Touch();
    }

    public void RemoveItem(Guid variantId)
    {
        _items.Remove(RequireItem(variantId));
        Touch();
    }

    public void ApplyCoupon(string code)
    {
        CouponCode = Ensure.MaxLength(Ensure.NotBlank(code), 32).ToUpperInvariant();
        Touch();
    }

    public void ClearCoupon()
    {
        CouponCode = null;
        Touch();
    }

    public CartReservation AttachReservation(Guid inventoryReservationId, Guid variantId, DateTimeOffset expiresAt)
    {
        var reservation = CartReservation.Create(Id, inventoryReservationId, variantId, expiresAt);
        _reservations.Add(reservation);
        Touch();
        return reservation;
    }

    public void Clear()
    {
        _items.Clear();
        _reservations.Clear();
        CouponCode = null;
        Touch();
    }

    private CartItem RequireItem(Guid variantId) =>
        _items.SingleOrDefault(i => i.VariantId == variantId)
        ?? throw new DomainValidationException("cart_item_missing", "Cart item was not found.");
}
