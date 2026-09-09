using SokoHub.Domain.Events.Payments;

namespace SokoHub.Domain.Modules.Payments;

public sealed class Payment : AggregateRoot
{
    private readonly List<PaymentAttempt> _attempts = [];
    private readonly List<PaymentCallback> _callbacks = [];
    private readonly List<PaymentRefund> _refunds = [];

    private Payment()
    {
    }

    private Payment(Guid id, Guid orderId, Guid customerId, Money amount, PaymentMethod method, PaymentReference reference)
        : base(id)
    {
        OrderId = orderId;
        CustomerId = customerId;
        Amount = amount;
        Method = method;
        Reference = reference;
        Status = PaymentStatus.Pending;
    }

    public Guid OrderId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Money Amount { get; private set; }

    public PaymentMethod Method { get; private set; }

    public PaymentReference Reference { get; private set; } = null!;

    public PaymentStatus Status { get; private set; }

    public IReadOnlyList<PaymentAttempt> Attempts => _attempts.AsReadOnly();

    public IReadOnlyList<PaymentCallback> Callbacks => _callbacks.AsReadOnly();

    public IReadOnlyList<PaymentRefund> Refunds => _refunds.AsReadOnly();

    public Money RefundedTotal => _refunds
        .Where(r => r.IsSucceeded)
        .Aggregate(Money.Zero(Amount.Currency), (sum, refund) => sum + refund.Amount);

    public static Payment Create(
        Guid orderId,
        Guid customerId,
        Money amount,
        PaymentMethod method,
        Guid? id = null)
    {
        Ensure.That(!amount.IsZero, "payment_amount", "Payment amount must be greater than zero.");
        var paymentId = id ?? Guid.CreateVersion7();
        return new Payment(
            paymentId,
            Ensure.NotEmpty(orderId),
            Ensure.NotEmpty(customerId),
            amount,
            method,
            PaymentReference.Next(paymentId));
    }

    public PaymentIntent InitiateMpesaStk(PhoneNumber phone, string checkoutRequestId, string merchantRequestId)
    {
        Ensure.That(Method is PaymentMethod.MpesaStk or PaymentMethod.MpesaC2B, "payment_method", "STK initiation requires an M-Pesa method.");
        Ensure.That(Status is PaymentStatus.Pending or PaymentStatus.Failed, "payment_not_pending", "Payment cannot be initiated in its current state.");
        var intent = PaymentIntent.Create(Id, phone, Ensure.NotBlank(checkoutRequestId), Ensure.NotBlank(merchantRequestId));
        _attempts.Add(PaymentAttempt.Start(Id, Method, checkoutRequestId));
        Status = PaymentStatus.Initiated;
        IncrementVersion();
        Raise(new PaymentInitiatedEvent
        {
            AggregateId = Id,
            OrderId = OrderId,
            Amount = Amount,
            Method = Method.ToString(),
            CheckoutRequestId = checkoutRequestId
        });
        return intent;
    }

    public void ApplyCallback(string checkoutRequestId, int resultCode, string resultDescription, string? mpesaReceiptNumber, Money? paidAmount)
    {
        Ensure.That(Status == PaymentStatus.Initiated, "payment_not_initiated", "Callback can only be applied to an initiated payment.");
        Ensure.That(
            _callbacks.TrueForAll(c => c.CheckoutRequestId != checkoutRequestId || c.MpesaReceiptNumber != mpesaReceiptNumber),
            "payment_duplicate_callback",
            "Duplicate M-Pesa callback.");

        var callback = PaymentCallback.Record(Id, checkoutRequestId, resultCode, resultDescription, mpesaReceiptNumber, paidAmount);
        _callbacks.Add(callback);

        var attempt = _attempts.LastOrDefault(a => a.CheckoutRequestId == checkoutRequestId);
        attempt?.Complete(resultCode == 0);

        if (resultCode == 0)
        {
            if (paidAmount is { } actual)
            {
                Ensure.That(actual.Currency == Amount.Currency, "currency_mismatch", "Callback amount currency mismatch.");
                Ensure.That(actual.Amount == Amount.Amount, "payment_amount_mismatch", "Paid amount does not match the payment.");
            }

            Status = PaymentStatus.Succeeded;
            IncrementVersion();
            Raise(new PaymentSucceededEvent
            {
                AggregateId = Id,
                OrderId = OrderId,
                Amount = Amount,
                Reference = mpesaReceiptNumber ?? Reference.Value
            });
            return;
        }

        Status = PaymentStatus.Failed;
        IncrementVersion();
        Raise(new PaymentFailedEvent
        {
            AggregateId = Id,
            OrderId = OrderId,
            Reason = resultDescription,
            ResultCode = resultCode.ToString()
        });
    }

    public PaymentRefund Refund(Money amount, string reason)
    {
        Ensure.That(Status is PaymentStatus.Succeeded or PaymentStatus.PartiallyRefunded, "payment_not_refundable", "Only successful payments can be refunded.");
        Ensure.That(amount.Currency == Amount.Currency, "currency_mismatch", "Refund currency must match the payment.");
        Ensure.That(RefundedTotal + amount <= Amount, "refund_exceeds_payment", "Refund exceeds captured amount.");
        var refund = PaymentRefund.Create(Id, amount, reason);
        _refunds.Add(refund);
        Status = RefundedTotal + amount == Amount ? PaymentStatus.Refunded : PaymentStatus.PartiallyRefunded;
        IncrementVersion();
        Raise(new PaymentRefundedEvent
        {
            AggregateId = Id,
            OrderId = OrderId,
            Amount = amount,
            Reason = reason
        });
        return refund;
    }

    public void Cancel(string reason)
    {
        Ensure.That(Status is PaymentStatus.Pending or PaymentStatus.Initiated, "payment_not_cancellable", "Payment cannot be cancelled.");
        Ensure.NotBlank(reason);
        Status = PaymentStatus.Cancelled;
        IncrementVersion();
    }
}
