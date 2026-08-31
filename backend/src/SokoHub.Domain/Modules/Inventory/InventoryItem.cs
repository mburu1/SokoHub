using SokoHub.Domain.Events.Inventory;

namespace SokoHub.Domain.Modules.Inventory;

public sealed class InventoryItem : AggregateRoot
{
    private readonly List<InventoryReservation> _reservations = [];
    private readonly List<InventoryAdjustment> _adjustments = [];
    private readonly List<StockLedgerEntry> _ledger = [];

    private InventoryItem()
    {
    }

    private InventoryItem(Guid id, Guid warehouseId, Guid variantId, Sku sku, int onHand)
        : base(id)
    {
        WarehouseId = warehouseId;
        VariantId = variantId;
        Sku = sku;
        OnHand = onHand;
        Reserved = 0;
    }

    public Guid WarehouseId { get; private set; }

    public Guid VariantId { get; private set; }

    public Sku Sku { get; private set; } = null!;

    public int OnHand { get; private set; }

    public int Reserved { get; private set; }

    public int Available => OnHand - Reserved;

    public IReadOnlyList<InventoryReservation> Reservations => _reservations.AsReadOnly();

    public IReadOnlyList<InventoryAdjustment> Adjustments => _adjustments.AsReadOnly();

    public IReadOnlyList<StockLedgerEntry> Ledger => _ledger.AsReadOnly();

    public static InventoryItem Open(Guid warehouseId, Guid variantId, Sku sku, int onHand = 0, Guid? id = null)
    {
        Ensure.NotNegative(onHand);
        var item = new InventoryItem(id ?? Guid.Empty, Ensure.NotEmpty(warehouseId), Ensure.NotEmpty(variantId), sku, onHand);
        if (onHand > 0)
        {
            item._ledger.Add(StockLedgerEntry.Record(item.Id, onHand, "opening_balance"));
        }

        return item;
    }

    public InventoryReservation Reserve(Guid ownerId, int quantity, DateTimeOffset expiresAt)
    {
        Ensure.Positive(quantity);
        Ensure.That(Available >= quantity, "insufficient_stock", $"Only {Available} units available for {Sku}.");
        var reservation = InventoryReservation.Create(Id, ownerId, quantity, expiresAt);
        _reservations.Add(reservation);
        Reserved += quantity;
        _ledger.Add(StockLedgerEntry.Record(Id, -quantity, "reserve"));
        IncrementVersion();
        Raise(new InventoryReservedEvent
        {
            AggregateId = Id,
            VariantId = VariantId,
            WarehouseId = WarehouseId,
            Quantity = quantity,
            ReservationId = reservation.Id
        });
        return reservation;
    }

    public void Consume(Guid reservationId)
    {
        var reservation = RequireReservation(reservationId);
        reservation.Consume();
        Reserved -= reservation.Quantity;
        OnHand -= reservation.Quantity;
        _ledger.Add(StockLedgerEntry.Record(Id, -reservation.Quantity, "consume"));
        IncrementVersion();
        if (Available == 0)
        {
            Raise(new InventoryDepletedEvent
            {
                AggregateId = Id,
                VariantId = VariantId,
                WarehouseId = WarehouseId
            });
        }
    }

    public void Release(Guid reservationId)
    {
        var reservation = RequireReservation(reservationId);
        reservation.Release();
        Reserved -= reservation.Quantity;
        _ledger.Add(StockLedgerEntry.Record(Id, reservation.Quantity, "release"));
        IncrementVersion();
        Raise(new InventoryReleasedEvent
        {
            AggregateId = Id,
            VariantId = VariantId,
            ReservationId = reservationId,
            Quantity = reservation.Quantity
        });
    }

    public void Expire(Guid reservationId)
    {
        var reservation = RequireReservation(reservationId);
        reservation.Expire();
        Reserved -= reservation.Quantity;
        IncrementVersion();
        Raise(new InventoryReleasedEvent
        {
            AggregateId = Id,
            VariantId = VariantId,
            ReservationId = reservationId,
            Quantity = reservation.Quantity
        });
    }

    public void Adjust(int delta, AdjustmentReason reason, string note)
    {
        Ensure.That(delta != 0, "adjustment_zero", "Adjustment delta cannot be zero.");
        var next = OnHand + delta;
        Ensure.That(next >= Reserved, "adjustment_below_reserved", "On-hand cannot fall below reserved quantity.");
        OnHand = next;
        _adjustments.Add(InventoryAdjustment.Create(Id, delta, reason, note));
        _ledger.Add(StockLedgerEntry.Record(Id, delta, reason.ToString()));
        IncrementVersion();
        Raise(new InventoryAdjustedEvent
        {
            AggregateId = Id,
            VariantId = VariantId,
            Delta = delta,
            OnHand = OnHand,
            Reason = reason.ToString()
        });
        if (Available == 0)
        {
            Raise(new InventoryDepletedEvent
            {
                AggregateId = Id,
                VariantId = VariantId,
                WarehouseId = WarehouseId
            });
        }
    }

    private InventoryReservation RequireReservation(Guid reservationId) =>
        _reservations.SingleOrDefault(r => r.Id == reservationId)
        ?? throw new DomainValidationException("reservation_missing", "Reservation was not found.");
}
