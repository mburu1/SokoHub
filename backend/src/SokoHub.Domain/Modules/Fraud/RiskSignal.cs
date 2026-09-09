using SokoHub.Domain.Common.Entities;

namespace SokoHub.Domain.Modules.Fraud;

public class RiskSignal : Entity
{
    public Guid CaseId { get; private set; }
    public string SignalType { get; private set; } // e.g. "IP_Mismatch", "Velocity_High"
    public int RiskWeight { get; private set; }
    public string Evidence { get; private set; }
    public DateTime DetectedAt { get; private set; }

    public RiskSignal(Guid id, Guid caseId, string signalType, int riskWeight, string evidence)
        : base(id)
    {
        CaseId = caseId;
        SignalType = signalType;
        RiskWeight = riskWeight;
        Evidence = evidence;
        DetectedAt = DateTime.UtcNow;
    }
}
