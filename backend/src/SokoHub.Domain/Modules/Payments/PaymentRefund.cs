namespace SokoHub.Domain.Modules.Payments;

public sealed class PaymentRefund : Entity
{
    private PaymentRefund()
    {
    }

    private PaymentRefund(Guid id, Guid paymentId, Money amount, string reason)
        : base(id)
    {
        PaymentId = paymentId;
        Amount = amount;
        Reason = reason;
        IsSucceeded = true;
    }

    public Guid PaymentId { get; private set; }

    public Money Amount { get; private set; }

    public string Reason { get; private set; } = string.Empty;

    public bool IsSucceeded { get; private set; }

    internal static PaymentRefund Create(Guid paymentId, Money amount, string reason)
    {
        Ensure.That(!amount.IsZero, "refund_amount", "Refund amount must be greater than zero.");
        return new PaymentRefund(Guid.Empty, paymentId, amount, Ensure.MaxLength(Ensure.NotBlank(reason), 500));
    }
}
