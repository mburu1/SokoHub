namespace SokoHub.Domain.Modules.Orders;

public sealed class VendorOrder : Entity
{
    private readonly List<Guid> _itemIds = [];

    private VendorOrder()
    {
    }

    private VendorOrder(Guid id, Guid orderId, Guid vendorId)
        : base(id)
    {
        OrderId = orderId;
        VendorId = vendorId;
        Status = VendorOrderStatus.Pending;
    }

    public Guid OrderId { get; private set; }

    public Guid VendorId { get; private set; }

    public VendorOrderStatus Status { get; private set; }

    public TrackingNumber? TrackingNumber { get; private set; }

    public IReadOnlyList<Guid> ItemIds => _itemIds.AsReadOnly();

    internal static VendorOrder Create(Guid orderId, Guid vendorId) =>
        new(Guid.Empty, orderId, Ensure.NotEmpty(vendorId));

    internal void AttachItem(Guid itemId) => _itemIds.Add(itemId);

    internal void Accept() => Status = VendorOrderStatus.Accepted;

    public void Pack()
    {
        Ensure.That(Status == VendorOrderStatus.Accepted, "vendor_order_not_accepted", "Vendor order must be accepted before packing.");
        Status = VendorOrderStatus.Packed;
        Touch();
    }

    internal void Ship(TrackingNumber trackingNumber)
    {
        Ensure.That(Status is VendorOrderStatus.Accepted or VendorOrderStatus.Packed, "vendor_order_not_shippable", "Vendor order cannot be shipped yet.");
        TrackingNumber = trackingNumber;
        Status = VendorOrderStatus.Shipped;
        Touch();
    }

    internal void Deliver()
    {
        Ensure.That(Status == VendorOrderStatus.Shipped, "vendor_order_not_shipped", "Vendor order must be shipped before delivery.");
        Status = VendorOrderStatus.Delivered;
        Touch();
    }

    internal void Cancel()
    {
        Ensure.That(Status is VendorOrderStatus.Pending or VendorOrderStatus.Accepted, "vendor_order_not_cancellable", "Vendor order cannot be cancelled.");
        Status = VendorOrderStatus.Cancelled;
        Touch();
    }
}
