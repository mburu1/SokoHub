namespace SokoHub.Domain.Modules.Payments;

public sealed class PaymentTransaction : Entity
{
    private PaymentTransaction()
    {
    }

    private PaymentTransaction(Guid id, Guid paymentId, string channel, string externalId, Money amount)
        : base(id)
    {
        PaymentId = paymentId;
        Channel = channel;
        ExternalId = externalId;
        Amount = amount;
    }

    public Guid PaymentId { get; private set; }

    public string Channel { get; private set; } = string.Empty;

    public string ExternalId { get; private set; } = string.Empty;

    public Money Amount { get; private set; }

    public static PaymentTransaction Capture(Guid paymentId, string channel, string externalId, Money amount) =>
        new(Guid.Empty, Ensure.NotEmpty(paymentId), Ensure.NotBlank(channel), Ensure.NotBlank(externalId), amount);
}
