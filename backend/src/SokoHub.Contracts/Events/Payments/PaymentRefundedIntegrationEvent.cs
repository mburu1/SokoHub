using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Payments;

public record PaymentRefundedIntegrationEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal RefundAmount,
    string Reason,
    string? RefundReference) : IntegrationEvent;
