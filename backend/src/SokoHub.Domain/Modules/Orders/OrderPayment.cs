namespace SokoHub.Domain.Modules.Orders;

public sealed class OrderPayment : Entity
{
    private OrderPayment()
    {
    }

    private OrderPayment(Guid id, Guid orderId, Guid paymentId, Money amount)
        : base(id)
    {
        OrderId = orderId;
        PaymentId = paymentId;
        Amount = amount;
    }

    public Guid OrderId { get; private set; }

    public Guid PaymentId { get; private set; }

    public Money Amount { get; private set; }

    internal static OrderPayment Create(Guid orderId, Guid paymentId, Money amount) =>
        new(Guid.Empty, orderId, Ensure.NotEmpty(paymentId), amount);
}
