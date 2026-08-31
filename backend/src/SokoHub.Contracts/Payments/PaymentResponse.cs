namespace SokoHub.Contracts.Payments;

public record PaymentResponse(
    Guid PaymentId,
    string Reference,
    string Status,
    decimal Amount);
