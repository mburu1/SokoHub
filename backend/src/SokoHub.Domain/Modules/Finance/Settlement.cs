using SokoHub.Domain.Common.AggregateRoots;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Finance;

public class Settlement : AggregateRoot
{
    public Guid VendorId { get; private set; }
    public Money TotalAmount { get; private set; }
    public DateTime SettlementDate { get; private set; }
    public string Reference { get; private set; } = null!;
    public bool IsProcessed { get; private set; }

    private Settlement() { }

    public Settlement(Guid id, Guid vendorId, Money totalAmount, string reference)
        : base(id)
    {
        VendorId = vendorId;
        TotalAmount = totalAmount;
        Reference = reference;
        SettlementDate = DateTime.UtcNow;
        IsProcessed = false;
    }

    public void MarkAsProcessed()
    {
        IsProcessed = true;
        Touch();
    }
}
