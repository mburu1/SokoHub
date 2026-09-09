using SokoHub.Domain.Common.AggregateRoots;

namespace SokoHub.Domain.Modules.Fraud;

public class FraudRule : AggregateRoot
{
    public string RuleName { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public int RiskScore { get; private set; }
    public bool IsActive { get; private set; }

    private FraudRule() { }

    public FraudRule(Guid id, string ruleName, string description, int riskScore)
        : base(id)
    {
        RuleName = ruleName;
        Description = description;
        RiskScore = riskScore;
        IsActive = true;
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;
}
