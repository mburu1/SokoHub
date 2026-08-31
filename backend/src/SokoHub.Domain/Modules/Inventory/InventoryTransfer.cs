namespace SokoHub.Domain.Modules.Inventory;

public sealed class InventoryTransfer : AggregateRoot
{
    private InventoryTransfer()
    {
    }

    private InventoryTransfer(Guid id, Guid sourceWarehouseId, Guid destinationWarehouseId, Guid variantId, int quantity)
        : base(id)
    {
        SourceWarehouseId = sourceWarehouseId;
        DestinationWarehouseId = destinationWarehouseId;
        VariantId = variantId;
        Quantity = quantity;
        Status = TransferStatus.Pending;
    }

    public Guid SourceWarehouseId { get; private set; }

    public Guid DestinationWarehouseId { get; private set; }

    public Guid VariantId { get; private set; }

    public int Quantity { get; private set; }

    public TransferStatus Status { get; private set; }

    public static InventoryTransfer Request(Guid sourceWarehouseId, Guid destinationWarehouseId, Guid variantId, int quantity)
    {
        Ensure.That(sourceWarehouseId != destinationWarehouseId, "transfer_same_warehouse", "Source and destination warehouses must differ.");
        return new InventoryTransfer(
            Guid.Empty,
            Ensure.NotEmpty(sourceWarehouseId),
            Ensure.NotEmpty(destinationWarehouseId),
            Ensure.NotEmpty(variantId),
            Ensure.Positive(quantity));
    }

    public void Complete()
    {
        Ensure.That(Status == TransferStatus.Pending, "transfer_not_pending", "Transfer is not pending.");
        Status = TransferStatus.Completed;
        IncrementVersion();
    }

    public void Cancel()
    {
        Ensure.That(Status == TransferStatus.Pending, "transfer_not_pending", "Transfer is not pending.");
        Status = TransferStatus.Cancelled;
        IncrementVersion();
    }
}

public enum TransferStatus
{
    Pending = 0,
    Completed = 1,
    Cancelled = 2
}
