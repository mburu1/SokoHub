using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Payments;

public record PaymentFailedIntegrationEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string FailureReason,
    string? ErrorCode) : IntegrationEvent;
