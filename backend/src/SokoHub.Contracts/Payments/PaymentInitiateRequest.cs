namespace SokoHub.Contracts.Payments;

public record PaymentInitiateRequest(
    Guid OrderId,
    string PhoneNumber,
    decimal Amount);
