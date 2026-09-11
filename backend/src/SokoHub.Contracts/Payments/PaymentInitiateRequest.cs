namespace SokoHub.Contracts.Payments;

public record PaymentInitiateRequest(
    Guid OrderId,
    string PhoneNumber,
    decimal Amount,
    string PaymentMethod = "Mpesa",
    string Currency = "KES");
