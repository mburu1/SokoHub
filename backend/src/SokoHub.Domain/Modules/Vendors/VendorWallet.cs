using SokoHub.Domain.Common;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Vendors;

public sealed class VendorWallet : Entity
{
    private VendorWallet()
    {
    }

    public VendorWallet(Guid id, Guid vendorId, string mpesaPhoneNumber)
        : base(id)
    {
        VendorId = vendorId;
        MpesaPhoneNumber = Ensure.NotBlank(mpesaPhoneNumber);
        Balance = Money.Zero;
        Currency = "KES";
    }

    public Guid VendorId { get; private set; } = null!;
    public string MpesaPhoneNumber { get; private set; } = string.Empty;
    public Money Balance { get; private set; } = null!;
    public string Currency { get; private set; } = "KES";

    public void Credit(Money amount)
    {
        Balance = Balance.Add(amount);
    }

    public void Debit(Money amount)
    {
        Ensure.That(Balance.Amount >= amount.Amount, "insufficient_funds", "Insufficient wallet balance.");
        Balance = Balance.Subtract(amount);
    }
}
