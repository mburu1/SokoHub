using SokoHub.Domain.Common.Entities;

namespace SokoHub.Domain.Modules.Disputes;

public class DisputeEvidence : Entity
{
    public Guid DisputeId { get; private set; }
    public string EvidenceType { get; private set; } // e.g. "Image", "Text", "Document"
    public string ContentUrl { get; private set; }
    public string Description { get; private set; }
    public DateTime UploadedAt { get; private set; }

    public DisputeEvidence(Guid id, Guid disputeId, string evidenceType, string contentUrl, string description)
        : base(id)
    {
        DisputeId = disputeId;
        EvidenceType = evidenceType;
        ContentUrl = contentUrl;
        Description = description;
        UploadedAt = DateTime.UtcNow;
    }
}
