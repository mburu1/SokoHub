namespace SokoHub.Contracts.Payments;

public record PaymentResponse(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    string Currency,
    string Method,
    string Status,
    string Reference);
