namespace SokoHub.Contracts.Payments;

public record PaymentCallbackRequest(
    string CheckoutRequestId,
    int ResultCode,
    string ResultDescription,
    string? MpesaReceiptNumber,
    decimal? Amount);
