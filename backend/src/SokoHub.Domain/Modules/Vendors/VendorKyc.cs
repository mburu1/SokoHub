using SokoHub.Domain.Common;
using SokoHub.Domain.Enums;

namespace SokoHub.Domain.Modules.Vendors;

public sealed class VendorKyc : Entity
{
    private VendorKyc()
    {
    }

    public VendorKyc(Guid id, Guid vendorId, VendorStatus initialStatus)
        : base(id)
    {
        VendorId = vendorId;
        Status = initialStatus;
        SubmittedAt = DateTimeOffset.UtcNow;
    }

    public Guid VendorId { get; private set; } = null!;
    public VendorStatus Status { get; private set; }
    public DateTimeOffset SubmittedAt { get; private set; }
    public DateTimeOffset? VerifiedAt { get; private set; }
    public string? VerifiedBy { get; private set; }
    public string? RejectionReason { get; private set; }

    public void MarkAsVerified(string verifiedBy)
    {
        Status = VendorStatus.Active;
        VerifiedAt = DateTimeOffset.UtcNow;
        VerifiedBy = verifiedBy;
        RejectionReason = null;
    }

    public void MarkAsRejected(string reason)
    {
        Status = VendorStatus.Rejected;
        RejectionReason = reason;
        VerifiedAt = null;
        VerifiedBy = null;
    }
}
