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
        Touch();
    }

    public Guid UserId { get; private set; } = Guid.Empty;
    public string BusinessName { get; private set; } = string.Empty;
    public KraPin TaxId { get; private set; } = null!;
    public Percentage CommissionRate { get; private set; } = Percentage.Zero;
    public VendorStatus Status { get; private set; }

    public IReadOnlyList<VendorDocument> Documents => _documents.AsReadOnly();
    public IReadOnlyList<VendorSettlement> Settlements => _settlements.AsReadOnly();

    public static Vendor Register(Guid userId, string businessName, KraPin taxId, Percentage commissionRate, Guid? id = null) =>
        new(
            id ?? Guid.NewGuid(),
            userId,
            Ensure.MaxLength(Ensure.NotBlank(businessName), 200),
            taxId,
            commissionRate);

    public void UpdateProfile(string businessName)
    {
        BusinessName = Ensure.MaxLength(Ensure.NotBlank(businessName), 200);
        Touch();
    }

    public void UpdateCommission(Percentage commissionRate)
    {
        CommissionRate = commissionRate;
        Touch();
    }

    public void VerifyKyc(Guid documentId, string verifiedBy)
    {
        var document = SokoHub.Domain.Modules.Vendors.VendorDocument.Create(this.Id, documentId, verifiedBy);
        _documents.Add(document);
        SetUnderReview();
        Touch();
    }

    public void SetUnderReview()
    {
        Status = VendorStatus.UnderReview;
        Touch();
    }

    public void AddSettlement(VendorSettlement settlement)
    {
        _settlements.Add(settlement);
        Touch();
    }

    public void Suspend(string reason)
    {
        Ensure.NotBlank(reason);
        Status = VendorStatus.Suspended;
        Touch();
    }

    public void Reject(string reason)
    {
        Ensure.NotBlank(reason);
        Status = VendorStatus.Rejected;
        Touch();
    }

    public void AddDocument(VendorDocument document)
    {
        _documents.Add(document);
        Touch();
    }
}
