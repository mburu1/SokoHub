using SokoHub.Domain.Events.Orders;

namespace SokoHub.Domain.Modules.Orders;

public sealed class Order : AggregateRoot
{
    private readonly List<OrderItem> _items = [];
    private readonly List<VendorOrder> _vendorOrders = [];
    private readonly List<OrderNote> _notes = [];
    private readonly List<OrderPayment> _payments = [];
    private readonly List<OrderStatusHistory> _statusHistory = [];

    private Order()
    {
    }

    private Order(Guid id, Guid customerId, OrderNumber number, Address shippingAddress)
        : base(id)
    {
        CustomerId = customerId;
        Number = number;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.PendingPayment;
        Currency = Money.DefaultCurrency;
    }

    public Guid CustomerId { get; private set; }

    public OrderNumber Number { get; private set; } = null!;

    public OrderStatus Status { get; private set; }

    public Address ShippingAddress { get; private set; } = null!;

    public string Currency { get; private set; } = Money.DefaultCurrency;

    public Money Subtotal { get; private set; }

    public Money ShippingTotal { get; private set; }

    public Money DiscountTotal { get; private set; }

    public Money TaxTotal { get; private set; }

    public Money GrandTotal { get; private set; }

    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    public IReadOnlyList<VendorOrder> VendorOrders => _vendorOrders.AsReadOnly();

    public IReadOnlyList<OrderNote> Notes => _notes.AsReadOnly();

    public IReadOnlyList<OrderPayment> Payments => _payments.AsReadOnly();

    public IReadOnlyList<OrderStatusHistory> StatusHistory => _statusHistory.AsReadOnly();

    public static Order Place(
        Guid customerId,
        OrderNumber number,
        Address shippingAddress,
        IReadOnlyList<OrderLineDraft> lines,
        Money shippingTotal,
        Money discountTotal,
        Percentage taxRate,
        Guid? id = null)
    {
        Ensure.That(lines.Count > 0, "order_empty", "An order must contain at least one item.");
        var order = new Order(id ?? Guid.Empty, Ensure.NotEmpty(customerId), number, shippingAddress)
        {
            ShippingTotal = shippingTotal,
            DiscountTotal = discountTotal
        };

        foreach (var line in lines)
        {
            order._items.Add(OrderItem.Create(order.Id, line));
        }

        order.SplitByVendor();
        order.Recalculate(taxRate);
        order.RecordStatus(OrderStatus.PendingPayment, "Order placed.");
        order.Raise(new OrderCreatedEvent
        {
            AggregateId = order.Id,
            CustomerId = order.CustomerId,
            OrderNumber = order.Number.Value,
            GrandTotal = order.GrandTotal
        });
        return order;
    }

    public void Confirm()
    {
        Ensure.That(Status == OrderStatus.PendingPayment, "order_not_pending", "Only pending orders can be confirmed.");
        Status = OrderStatus.Confirmed;
        foreach (var vendorOrder in _vendorOrders)
        {
            vendorOrder.Accept();
        }

        RecordStatus(Status, "Payment confirmed.");
        IncrementVersion();
        Raise(new OrderConfirmedEvent
        {
            AggregateId = Id,
            CustomerId = CustomerId,
            OrderNumber = Number.Value
        });
    }

    public void Cancel(string reason)
    {
        Ensure.That(Status is OrderStatus.PendingPayment or OrderStatus.Confirmed, "order_not_cancellable", "Order cannot be cancelled in its current state.");
        var trimmed = Ensure.MaxLength(Ensure.NotBlank(reason), 500);
        Status = OrderStatus.Cancelled;
        foreach (var vendorOrder in _vendorOrders)
        {
            vendorOrder.Cancel();
        }

        RecordStatus(Status, trimmed);
        IncrementVersion();
        Raise(new OrderCancelledEvent
        {
            AggregateId = Id,
            OrderNumber = Number.Value,
            Reason = trimmed
        });
    }

    public void MarkVendorShipped(Guid vendorOrderId, TrackingNumber trackingNumber)
    {
        var vendorOrder = RequireVendorOrder(vendorOrderId);
        vendorOrder.Ship(trackingNumber);
        Status = _vendorOrders.TrueForAll(v => v.Status is VendorOrderStatus.Shipped or VendorOrderStatus.Delivered)
            ? OrderStatus.Shipped
            : OrderStatus.PartiallyShipped;
        RecordStatus(Status, $"Vendor order shipped ({trackingNumber}).");
        IncrementVersion();
        Raise(new OrderShippedEvent
        {
            AggregateId = Id,
            VendorOrderId = vendorOrderId,
            TrackingNumber = trackingNumber.Value
        });
    }

    public void MarkVendorDelivered(Guid vendorOrderId)
    {
        var vendorOrder = RequireVendorOrder(vendorOrderId);
        vendorOrder.Deliver();
        if (_vendorOrders.TrueForAll(v => v.Status == VendorOrderStatus.Delivered))
        {
            Status = OrderStatus.Delivered;
        }

        RecordStatus(Status, "Vendor order delivered.");
        IncrementVersion();
        Raise(new OrderDeliveredEvent { AggregateId = Id, VendorOrderId = vendorOrderId });
    }

    public OrderNote AddNote(string body, Guid authorId, bool isInternal)
    {
        var note = OrderNote.Create(Id, body, authorId, isInternal);
        _notes.Add(note);
        Touch();
        return note;
    }

    public OrderPayment RecordPayment(Guid paymentId, Money amount)
    {
        Ensure.That(amount.Currency == Currency, "currency_mismatch", "Payment currency must match the order.");
        var payment = OrderPayment.Create(Id, paymentId, amount);
        _payments.Add(payment);
        Touch();
        return payment;
    }

    private void SplitByVendor()
    {
        foreach (var group in _items.GroupBy(i => i.VendorId))
        {
            var vendorOrder = VendorOrder.Create(Id, group.Key);
            foreach (var item in group)
            {
                vendorOrder.AttachItem(item.Id);
            }

            _vendorOrders.Add(vendorOrder);
        }
    }

    private void Recalculate(Percentage taxRate)
    {
        Subtotal = _items.Aggregate(Money.Zero(Currency), (sum, item) => sum + item.LineTotal);
        TaxTotal = taxRate.Of(Subtotal - DiscountTotal);
        GrandTotal = Subtotal + ShippingTotal + TaxTotal - DiscountTotal;
    }

    private void RecordStatus(OrderStatus status, string note)
    {
        _statusHistory.Add(OrderStatusHistory.Create(Id, status, note));
    }

    private VendorOrder RequireVendorOrder(Guid vendorOrderId) =>
        _vendorOrders.SingleOrDefault(v => v.Id == vendorOrderId)
        ?? throw new DomainValidationException("vendor_order_missing", "Vendor order was not found on this order.");
}

public sealed record OrderLineDraft(
    Guid VendorId,
    Guid ProductId,
    Guid VariantId,
    Sku Sku,
    string ProductName,
    Money UnitPrice,
    int Quantity);
