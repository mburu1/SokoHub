using SokoHub.Domain.Common.Entities;

namespace SokoHub.Domain.Modules.Disputes;

public class DisputeResolution : Entity
{
    public Guid DisputeId { get; private set; }
    public string ResolutionDetails { get; private set; }
    public decimal RefundAmount { get; private set; }
    public DateTime ResolvedAt { get; private set; }

    public DisputeResolution(Guid id, Guid disputeId, string resolutionDetails, decimal refundAmount)
        : base(id)
    {
        DisputeId = disputeId;
        ResolutionDetails = resolutionDetails;
        RefundAmount = refundAmount;
        ResolvedAt = DateTime.UtcNow;
    }
}
