namespace SokoHub.Domain.Modules.Payments;

public sealed class PaymentIntent : Entity
{
    private PaymentIntent()
    {
    }

    private PaymentIntent(Guid id, Guid paymentId, PhoneNumber phone, string checkoutRequestId, string merchantRequestId)
        : base(id)
    {
        PaymentId = paymentId;
        Phone = phone;
        CheckoutRequestId = checkoutRequestId;
        MerchantRequestId = merchantRequestId;
    }

    public Guid PaymentId { get; private set; }

    public PhoneNumber Phone { get; private set; } = null!;

    public string CheckoutRequestId { get; private set; } = string.Empty;

    public string MerchantRequestId { get; private set; } = string.Empty;

    internal static PaymentIntent Create(Guid paymentId, PhoneNumber phone, string checkoutRequestId, string merchantRequestId) =>
        new(Guid.Empty, paymentId, phone, checkoutRequestId, merchantRequestId);
}
