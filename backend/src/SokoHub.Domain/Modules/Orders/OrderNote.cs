namespace SokoHub.Domain.Modules.Orders;

public sealed class OrderNote : Entity
{
    private OrderNote()
    {
    }

    private OrderNote(Guid id, Guid orderId, string body, Guid authorId, bool isInternal)
        : base(id)
    {
        OrderId = orderId;
        Body = body;
        AuthorId = authorId;
        IsInternal = isInternal;
    }

    public Guid OrderId { get; private set; }

    public string Body { get; private set; } = string.Empty;

    public Guid AuthorId { get; private set; }

    public bool IsInternal { get; private set; }

    internal static OrderNote Create(Guid orderId, string body, Guid authorId, bool isInternal) =>
        new(Guid.Empty, orderId, Ensure.MaxLength(Ensure.NotBlank(body), 2000), Ensure.NotEmpty(authorId), isInternal);
}
