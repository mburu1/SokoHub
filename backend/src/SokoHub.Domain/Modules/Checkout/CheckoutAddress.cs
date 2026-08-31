namespace SokoHub.Domain.Modules.Checkout;

public sealed class CheckoutAddress : Entity
{
    private CheckoutAddress()
    {
    }

    private CheckoutAddress(Guid id, Guid checkoutSessionId, Address address, PhoneNumber phone, string recipientName)
        : base(id)
    {
        CheckoutSessionId = checkoutSessionId;
        Address = address;
        Phone = phone;
        RecipientName = recipientName;
    }

    public Guid CheckoutSessionId { get; private set; }

    public Address Address { get; private set; } = null!;

    public PhoneNumber Phone { get; private set; } = null!;

    public string RecipientName { get; private set; } = string.Empty;

    public static CheckoutAddress Create(Guid checkoutSessionId, Address address, PhoneNumber phone, string recipientName) =>
        new(
            Guid.Empty,
            checkoutSessionId,
            address,
            phone,
            Ensure.MaxLength(Ensure.NotBlank(recipientName), 120));
}
