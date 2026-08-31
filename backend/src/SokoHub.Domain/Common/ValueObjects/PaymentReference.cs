namespace SokoHub.Domain.Common.ValueObjects;

public sealed record PaymentReference
{
    public string Value { get; }

    private PaymentReference(string value) => Value = value;

    public static PaymentReference Parse(string value)
    {
        var normalized = Ensure.NotBlank(value).ToUpperInvariant();
        Ensure.MaxLength(normalized, 64);
        return new PaymentReference(normalized);
    }

    public static PaymentReference FromMpesa(string checkoutRequestId)
    {
        return Parse($"MPESA-{Ensure.NotBlank(checkoutRequestId)}");
    }

    public static PaymentReference Next(Guid paymentId) =>
        new($"PAY-{paymentId.ToString("N")[..12].ToUpperInvariant()}");

    public override string ToString() => Value;

    public static implicit operator string(PaymentReference reference) => reference.Value;
}
