using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Payments;

public record PaymentSucceededIntegrationEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod,
    string TransactionReference,
    string? MpesaReceiptNumber) : IntegrationEvent;
