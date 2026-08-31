namespace SokoHub.Domain.Modules.Orders;

public sealed class OrderStatusHistory : Entity
{
    private OrderStatusHistory()
    {
    }

    private OrderStatusHistory(Guid id, Guid orderId, OrderStatus status, string note)
        : base(id)
    {
        OrderId = orderId;
        Status = status;
        Note = note;
    }

    public Guid OrderId { get; private set; }

    public OrderStatus Status { get; private set; }

    public string Note { get; private set; } = string.Empty;

    internal static OrderStatusHistory Create(Guid orderId, OrderStatus status, string note) =>
        new(Guid.Empty, orderId, status, Ensure.MaxLength(Ensure.NotBlank(note), 500));
}
