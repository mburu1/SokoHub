using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Payments;

public record PaymentInitiatedIntegrationEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string? CheckoutRequestId) : IntegrationEvent;
