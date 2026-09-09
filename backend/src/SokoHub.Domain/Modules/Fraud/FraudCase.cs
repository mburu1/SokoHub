using SokoHub.Domain.Common.AggregateRoots;
using SokoHub.Domain.Common.Entities;

namespace SokoHub.Domain.Modules.Fraud;

public class FraudCase : AggregateRoot
{
    public Guid OrderId { get; private set; }
    public Guid CustomerId { get; private set; }
    public FraudStatus Status { get; private set; }
    public string Analysis { get; private set; } = null!;
    private readonly List<RiskSignal> _signals = [];

    public IReadOnlyList<RiskSignal> Signals => _signals.AsReadOnly();

    private FraudCase() { }

    public FraudCase(Guid id, Guid orderId, Guid customerId, string analysis)
        : base(id)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Analysis = analysis;
        Status = FraudStatus.UnderReview;
        Touch();
    }

    public void AddSignal(RiskSignal signal)
    {
        _signals.Add(signal);
        Touch();
    }

    public void Resolve(FraudStatus finalStatus, string resolutionNotes)
    {
        Status = finalStatus;
        Analysis = resolutionNotes;
        Touch();
    }
}

public enum FraudStatus
{
    UnderReview,
    Cleared,
    Flagged,
    ConfirmedFraud
}
