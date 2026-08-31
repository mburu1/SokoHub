namespace SokoHub.Domain.Modules.Cart;

public sealed class CartReservation : Entity
{
    private CartReservation()
    {
    }

    private CartReservation(Guid id, Guid cartId, Guid inventoryReservationId, Guid variantId, DateTimeOffset expiresAt)
        : base(id)
    {
        CartId = cartId;
        InventoryReservationId = inventoryReservationId;
        VariantId = variantId;
        ExpiresAt = expiresAt;
    }

    public Guid CartId { get; private set; }

    public Guid InventoryReservationId { get; private set; }

    public Guid VariantId { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public bool IsExpired(DateTimeOffset now) => now >= ExpiresAt;

    internal static CartReservation Create(Guid cartId, Guid inventoryReservationId, Guid variantId, DateTimeOffset expiresAt) =>
        new(Guid.Empty, cartId, Ensure.NotEmpty(inventoryReservationId), Ensure.NotEmpty(variantId), expiresAt);
}
