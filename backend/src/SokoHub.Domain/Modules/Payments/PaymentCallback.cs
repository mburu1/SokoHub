namespace SokoHub.Domain.Modules.Payments;

public sealed class PaymentCallback : Entity
{
    private PaymentCallback()
    {
    }

    private PaymentCallback(
        Guid id,
        Guid paymentId,
        string checkoutRequestId,
        int resultCode,
        string resultDescription,
        string? mpesaReceiptNumber,
        Money? paidAmount)
        : base(id)
    {
        PaymentId = paymentId;
        CheckoutRequestId = checkoutRequestId;
        ResultCode = resultCode;
        ResultDescription = resultDescription;
        MpesaReceiptNumber = mpesaReceiptNumber;
        PaidAmount = paidAmount;
        ReceivedAt = DateTimeOffset.UtcNow;
    }

    public Guid PaymentId { get; private set; }

    public string CheckoutRequestId { get; private set; } = string.Empty;

    public int ResultCode { get; private set; }

    public string ResultDescription { get; private set; } = string.Empty;

    public string? MpesaReceiptNumber { get; private set; }

    public Money? PaidAmount { get; private set; }

    public DateTimeOffset ReceivedAt { get; private set; }

    internal static PaymentCallback Record(
        Guid paymentId,
        string checkoutRequestId,
        int resultCode,
        string resultDescription,
        string? mpesaReceiptNumber,
        Money? paidAmount) =>
        new(
            Guid.Empty,
            paymentId,
            Ensure.NotBlank(checkoutRequestId),
            resultCode,
            Ensure.MaxLength(Ensure.NotBlank(resultDescription), 500),
            string.IsNullOrWhiteSpace(mpesaReceiptNumber) ? null : mpesaReceiptNumber.Trim(),
            paidAmount);
}
