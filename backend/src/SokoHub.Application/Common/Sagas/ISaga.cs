namespace SokoHub.Application.Common.Sagas;

/// <summary>Marker for long-running process managers (checkout, payout, refund).</summary>
public interface ISaga
{
    Guid CorrelationId { get; }
}
