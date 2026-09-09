using SokoHub.Domain.Common.AggregateRoots;
using SokoHub.Domain.Common.Entities;

namespace SokoHub.Domain.Modules.Disputes;

public class Dispute : AggregateRoot
{
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid VendorId { get; private set; }
    public string Reason { get; private set; } = null!;
    public decimal DisputedAmount { get; private set; }
    public DisputeStatus Status { get; private set; }
    private readonly List<DisputeEvidence> _evidences = [];
    private readonly List<DisputeMessage> _messages = [];

    public IReadOnlyList<DisputeEvidence> Evidences => _evidences.AsReadOnly();
    public IReadOnlyList<DisputeMessage> Messages => _messages.AsReadOnly();

    private Dispute() { }

    public Dispute(Guid id, Guid orderId, Guid customerId, Guid vendorId, string reason, decimal disputedAmount)
        : base(id)
    {
        OrderId = orderId;
        CustomerId = customerId;
        VendorId = vendorId;
        Reason = reason;
        DisputedAmount = disputedAmount;
        Status = DisputeStatus.Open;
        Touch();
    }

    public void AddEvidence(DisputeEvidence evidence)
    {
        _evidences.Add(evidence);
        Touch();
    }

    public void AddMessage(DisputeMessage message)
    {
        _messages.Add(message);
        Touch();
    }

    public void Resolve(DisputeResolution resolution)
    {
        Status = DisputeStatus.Resolved;
        // Handle resolution logic here
        Touch();
    }
}

public enum DisputeStatus
{
    Open,
    UnderReview,
    Resolved,
    Closed
}
