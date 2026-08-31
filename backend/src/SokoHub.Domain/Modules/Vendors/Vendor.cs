using SokoHub.Domain.Common;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Enums;

namespace SokoHub.Domain.Modules.Vendors;

public sealed class Vendor : AggregateRoot
{
    private readonly List<VendorDocument> _documents = [];
    private readonly List<VendorSettlement> _settlements = [];

    private Vendor()
    {
    }

    private Vendor(Guid id, Guid userId, string businessName, KraPin taxId, Percentage commissionRate)
        : base(id)
    {
        UserId = userId;
        BusinessName = businessName;
        TaxId = taxId;
        CommissionRate = commissionRate;
        Status = VendorStatus.Pending;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid UserId { get; private set; } = null!;
    public string BusinessName { get; private set; } = string.Empty;
    public KraPin TaxId { get; private set; } = null!;
    public Percentage CommissionRate { get; private set; } = null!;
    public VendorStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }

    public IReadOnlyList<VendorDocument> Documents => _documents.AsReadOnly();
    public IReadOnlyList<VendorSettlement> Settlements => _settlements.AsReadOnly();

    public static Vendor Register(Guid userId, string businessName, KraPin taxId, Percentage commissionRate, Guid? id = null) =>
        new(
            id ?? Guid.Empty,
            userId,
            Ensure.MaxLength(Ensure.NotBlank(businessName), 200),
            taxId,
            commissionRate);

    public void VerifyKyc(Guid documentId, string verifiedBy)
    {
        Ensure.That(Status == VendorStatus.Pending || Status == VendorStatus.UnderReview, "vendor_not_verifiable", "Vendor is not in a verifiable state.");

        // Logic to mark as verified would typically happen here or via a separate service
        // For the entity, we update status.
        Status = VendorStatus.Active;
        Touch();
    }

    public void Suspend(string reason)
    {
        Ensure.NotBlank(reason);
        Status = VendorStatus.Suspended;
        Touch();
    }

    public void Reactivate()
    {
        Ensure.That(Status == VendorStatus.Suspended, "vendor_not_suspended", "Only suspended vendors can be reactivated.");
        Status = VendorStatus.Active;
        Touch();
    }

    public void Reject(string reason)
    {
        Ensure.NotBlank(reason);
        Status = VendorStatus.Rejected;
        Touch();
    }

    public void UpdateCommission(Percentage newRate)
    {
        CommissionRate = newRate;
        Touch();
    }

    public void AddDocument(VendorDocument document)
    {
        _documents.Add(document);
        Touch();
    }

    public void AddSettlement(VendorSettlement settlement)
    {
        _settlements.Add(settlement);
        Touch();
    }
}
