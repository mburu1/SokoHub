namespace SokoHub.Domain.Events.Payments;

public sealed record PaymentRefundedEvent : DomainEvent
{
    public required Guid OrderId { get; init; }

    public required Money Amount { get; init; }

    public required string Reason { get; init; }
}
