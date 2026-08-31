namespace SokoHub.Domain.Modules.Inventory;

public sealed class InventoryReservation : Entity
{
    private InventoryReservation()
    {
    }

    private InventoryReservation(Guid id, Guid inventoryItemId, Guid ownerId, int quantity, DateTimeOffset expiresAt)
        : base(id)
    {
        InventoryItemId = inventoryItemId;
        OwnerId = ownerId;
        Quantity = quantity;
        ExpiresAt = expiresAt;
        Status = InventoryReservationStatus.Active;
    }

    public Guid InventoryItemId { get; private set; }

    public Guid OwnerId { get; private set; }

    public int Quantity { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public InventoryReservationStatus Status { get; private set; }

    internal static InventoryReservation Create(Guid inventoryItemId, Guid ownerId, int quantity, DateTimeOffset expiresAt)
    {
        Ensure.That(expiresAt > DateTimeOffset.UtcNow, "reservation_expiry", "Reservation expiry must be in the future.");
        return new InventoryReservation(Guid.Empty, inventoryItemId, Ensure.NotEmpty(ownerId), Ensure.Positive(quantity), expiresAt);
    }

    internal void Consume()
    {
        EnsureActive();
        Status = InventoryReservationStatus.Consumed;
        Touch();
    }

    internal void Release()
    {
        EnsureActive();
        Status = InventoryReservationStatus.Released;
        Touch();
    }

    internal void Expire()
    {
        EnsureActive();
        Status = InventoryReservationStatus.Expired;
        Touch();
    }

    private void EnsureActive() =>
        Ensure.That(Status == InventoryReservationStatus.Active, "reservation_not_active", "Reservation is not active.");
}
