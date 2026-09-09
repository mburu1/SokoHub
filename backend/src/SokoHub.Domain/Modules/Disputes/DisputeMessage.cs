using SokoHub.Domain.Common.Entities;

namespace SokoHub.Domain.Modules.Disputes;

public class DisputeMessage : Entity
{
    public Guid DisputeId { get; private set; }
    public Guid SenderId { get; private set; }
    public string Message { get; private set; }
    public DateTime SentAt { get; private set; }

    public DisputeMessage(Guid id, Guid disputeId, Guid senderId, string message)
        : base(id)
    {
        DisputeId = disputeId;
        SenderId = senderId;
        Message = message;
        SentAt = DateTime.UtcNow;
    }
}
