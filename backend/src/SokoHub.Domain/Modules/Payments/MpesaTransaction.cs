namespace SokoHub.Domain.Modules.Payments;

public sealed class MpesaTransaction : AggregateRoot
{
    private MpesaTransaction()
    {
    }

    private MpesaTransaction(
        Guid id,
        Guid paymentId,
        string checkoutRequestId,
        string merchantRequestId,
        PhoneNumber phone,
        Money amount)
        : base(id)
    {
        PaymentId = paymentId;
        CheckoutRequestId = checkoutRequestId;
        MerchantRequestId = merchantRequestId;
        Phone = phone;
        Amount = amount;
        Status = MpesaTransactionStatus.Pending;
    }

    public Guid PaymentId { get; private set; }

    public string CheckoutRequestId { get; private set; } = string.Empty;

    public string MerchantRequestId { get; private set; } = string.Empty;

    public PhoneNumber Phone { get; private set; } = null!;

    public Money Amount { get; private set; }

    public string? ReceiptNumber { get; private set; }

    public MpesaTransactionStatus Status { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public static MpesaTransaction Start(
        Guid paymentId,
        string checkoutRequestId,
        string merchantRequestId,
        PhoneNumber phone,
        Money amount,
        Guid? id = null) =>
        new(
            id ?? Guid.Empty,
            Ensure.NotEmpty(paymentId),
            Ensure.NotBlank(checkoutRequestId),
            Ensure.NotBlank(merchantRequestId),
            phone,
            amount);

    public void Succeed(string receiptNumber)
    {
        Ensure.That(Status == MpesaTransactionStatus.Pending, "mpesa_not_pending", "Transaction is already terminal.");
        ReceiptNumber = Ensure.NotBlank(receiptNumber);
        Status = MpesaTransactionStatus.Succeeded;
        CompletedAt = DateTimeOffset.UtcNow;
        IncrementVersion();
    }

    public void Fail(string reason)
    {
        Ensure.NotBlank(reason);
        Ensure.That(Status == MpesaTransactionStatus.Pending, "mpesa_not_pending", "Transaction is already terminal.");
        Status = MpesaTransactionStatus.Failed;
        CompletedAt = DateTimeOffset.UtcNow;
        IncrementVersion();
    }

    public bool IsStale(DateTimeOffset now, TimeSpan timeout) =>
        Status == MpesaTransactionStatus.Pending && now - CreatedAt >= timeout;
}

public enum MpesaTransactionStatus
{
    Pending = 0,
    Succeeded = 1,
    Failed = 2
}
