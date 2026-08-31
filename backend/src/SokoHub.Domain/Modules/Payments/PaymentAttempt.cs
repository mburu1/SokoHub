namespace SokoHub.Domain.Modules.Payments;

public sealed class PaymentAttempt : Entity
{
    private PaymentAttempt()
    {
    }

    private PaymentAttempt(Guid id, Guid paymentId, PaymentMethod method, string checkoutRequestId)
        : base(id)
    {
        PaymentId = paymentId;
        Method = method;
        CheckoutRequestId = checkoutRequestId;
        StartedAt = DateTimeOffset.UtcNow;
    }

    public Guid PaymentId { get; private set; }

    public PaymentMethod Method { get; private set; }

    public string CheckoutRequestId { get; private set; } = string.Empty;

    public DateTimeOffset StartedAt { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public bool? Succeeded { get; private set; }

    internal static PaymentAttempt Start(Guid paymentId, PaymentMethod method, string checkoutRequestId) =>
        new(Guid.Empty, paymentId, method, checkoutRequestId);

    internal void Complete(bool succeeded)
    {
        Succeeded = succeeded;
        CompletedAt = DateTimeOffset.UtcNow;
        Touch();
    }
}
