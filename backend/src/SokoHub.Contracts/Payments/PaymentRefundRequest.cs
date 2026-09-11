namespace SokoHub.Contracts.Payments;

public record PaymentRefundRequest(
    decimal Amount,
    string Currency = "KES",
    string? Reason = null);
