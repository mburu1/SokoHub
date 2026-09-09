using SokoHub.Domain.Common.Entities;

namespace SokoHub.Domain.Modules.Fraud;

public class RiskAssessment : Entity
{
    public Guid CaseId { get; private set; }
    public int TotalRiskScore { get; private set; }
    public string AssessmentLevel { get; private set; } // e.g. "Low", "Medium", "High"
    public DateTime AssessedAt { get; private set; }

    public RiskAssessment(Guid id, Guid caseId, int totalRiskScore, string assessmentLevel)
        : base(id)
    {
        CaseId = caseId;
        TotalRiskScore = totalRiskScore;
        AssessmentLevel = assessmentLevel;
        AssessedAt = DateTime.UtcNow;
    }
}
