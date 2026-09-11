using SokoHub.Domain.Common;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Enums;

namespace SokoHub.Domain.Modules.Shipping;

public sealed class Shipment : Entity
{
    private Shipment() { }

    private Shipment(Guid id, Guid orderId, Guid vendorId, Guid courierId, Address origin, Address destination, Money cost)
        : base(id)
    {
        OrderId = orderId;
        VendorId = vendorId;
        CourierId = courierId;
        Origin = origin;
        Destination = destination;
        Cost = cost;
        Status = ShipmentStatus.Created;
    }

    public Guid OrderId { get; private set; }
    public Guid VendorId { get; private set; }
    public Guid CourierId { get; private set; }
    public Address Origin { get; private set; } = null!;
    public Address Destination { get; private set; } = null!;
    public Money Cost { get; private set; }
    public ShipmentStatus Status { get; private set; }
    public TrackingNumber? TrackingNumber { get; private set; }
    public DateTimeOffset? ShippedAt { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }

    public static Shipment Create(
        Guid orderId,
        Guid vendorId,
        Guid courierId,
        Address origin,
        Address destination,
        Money cost,
        Guid? id = null)
    {
        return new Shipment(id ?? Guid.NewGuid(), orderId, vendorId, courierId, origin, destination, cost);
    }

    public void AssignTracking(TrackingNumber trackingNumber)
    {
        TrackingNumber = trackingNumber;
        Status = ShipmentStatus.InTransit;
        ShippedAt = DateTimeOffset.UtcNow;
        Touch();
    }

    public void MarkDelivered()
    {
        Status = ShipmentStatus.Delivered;
        DeliveredAt = DateTimeOffset.UtcNow;
        Touch();
    }

    public void AssignCourier(Guid courierId)
    {
        CourierId = courierId;
        Touch();
    }
}