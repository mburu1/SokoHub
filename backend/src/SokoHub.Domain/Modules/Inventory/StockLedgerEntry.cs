namespace SokoHub.Domain.Modules.Inventory;

public sealed class StockLedgerEntry : Entity
{
    private StockLedgerEntry()
    {
    }

    private StockLedgerEntry(Guid id, Guid inventoryItemId, int quantityDelta, string reason)
        : base(id)
    {
        InventoryItemId = inventoryItemId;
        QuantityDelta = quantityDelta;
        Reason = reason;
    }

    public Guid InventoryItemId { get; private set; }

    public int QuantityDelta { get; private set; }

    public string Reason { get; private set; } = string.Empty;

    internal static StockLedgerEntry Record(Guid inventoryItemId, int quantityDelta, string reason) =>
        new(Guid.Empty, inventoryItemId, quantityDelta, Ensure.NotBlank(reason));
}
