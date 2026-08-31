namespace SokoHub.Domain.Modules.Checkout;

public sealed class CheckoutSession : AggregateRoot
{
    private readonly List<CheckoutItem> _items = [];

    private CheckoutSession()
    {
    }

    private CheckoutSession(Guid id, Guid customerId, Guid cartId)
        : base(id)
    {
        CustomerId = customerId;
        CartId = cartId;
        Status = CheckoutStatus.Open;
        Currency = Money.DefaultCurrency;
        ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30);
    }

    public Guid CustomerId { get; private set; }

    public Guid CartId { get; private set; }

    public Guid? OrderId { get; private set; }

    public CheckoutStatus Status { get; private set; }

    public CheckoutAddress? ShippingAddress { get; private set; }

    public string Currency { get; private set; } = Money.DefaultCurrency;

    public DateTimeOffset ExpiresAt { get; private set; }

    public IReadOnlyList<CheckoutItem> Items => _items.AsReadOnly();

    public Money Subtotal => _items.Aggregate(Money.Zero(Currency), (sum, item) => sum + item.LineTotal);

    public static CheckoutSession Start(Guid customerId, Guid cartId, IEnumerable<CheckoutItemDraft> items, Guid? id = null)
    {
        var session = new CheckoutSession(id ?? Guid.Empty, Ensure.NotEmpty(customerId), Ensure.NotEmpty(cartId));
        foreach (var draft in items)
        {
            session._items.Add(CheckoutItem.Create(session.Id, draft));
        }

        Ensure.That(session._items.Count > 0, "checkout_empty", "Checkout requires at least one item.");
        return session;
    }

    public void SetShippingAddress(CheckoutAddress address)
    {
        EnsureOpen();
        ShippingAddress = address;
        Touch();
    }

    public void MarkInventoryReserved()
    {
        EnsureOpen();
        Status = CheckoutStatus.InventoryReserved;
        IncrementVersion();
    }

    public void AwaitPayment()
    {
        Ensure.That(Status == CheckoutStatus.InventoryReserved, "checkout_not_reserved", "Inventory must be reserved before payment.");
        Ensure.That(ShippingAddress is not null, "checkout_no_address", "Shipping address is required.");
        Status = CheckoutStatus.AwaitingPayment;
        IncrementVersion();
    }

    public void Complete(Guid orderId)
    {
        Ensure.That(Status == CheckoutStatus.AwaitingPayment, "checkout_not_awaiting_payment", "Checkout is not awaiting payment.");
        OrderId = Ensure.NotEmpty(orderId);
        Status = CheckoutStatus.Completed;
        IncrementVersion();
    }

    public void Fail(string reason)
    {
        Ensure.NotBlank(reason);
        Ensure.That(Status is not CheckoutStatus.Completed, "checkout_already_completed", "Completed checkout cannot fail.");
        Status = CheckoutStatus.Failed;
        IncrementVersion();
    }

    public void Abandon()
    {
        Ensure.That(Status is CheckoutStatus.Open or CheckoutStatus.InventoryReserved or CheckoutStatus.AwaitingPayment, "checkout_not_abandonable", "Checkout cannot be abandoned.");
        Status = CheckoutStatus.Abandoned;
        IncrementVersion();
    }

    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt && Status is not CheckoutStatus.Completed;

    private void EnsureOpen() =>
        Ensure.That(Status == CheckoutStatus.Open, "checkout_not_open", "Checkout session is not open.");
}

public sealed record CheckoutItemDraft(
    Guid VendorId,
    Guid ProductId,
    Guid VariantId,
    Sku Sku,
    string Name,
    Money UnitPrice,
    int Quantity);
