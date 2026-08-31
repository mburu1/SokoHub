namespace SokoHub.Domain.Modules.Inventory;

public sealed class InventoryAdjustment : Entity
{
    private InventoryAdjustment()
    {
    }

    private InventoryAdjustment(Guid id, Guid inventoryItemId, int delta, AdjustmentReason reason, string note)
        : base(id)
    {
        InventoryItemId = inventoryItemId;
        Delta = delta;
        Reason = reason;
        Note = note;
    }

    public Guid InventoryItemId { get; private set; }

    public int Delta { get; private set; }

    public AdjustmentReason Reason { get; private set; }

    public string Note { get; private set; } = string.Empty;

    internal static InventoryAdjustment Create(Guid inventoryItemId, int delta, AdjustmentReason reason, string note) =>
        new(Guid.Empty, inventoryItemId, delta, reason, Ensure.MaxLength(Ensure.NotBlank(note), 500));
}
