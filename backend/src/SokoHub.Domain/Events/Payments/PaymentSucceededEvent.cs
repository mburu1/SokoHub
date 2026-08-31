namespace SokoHub.Domain.Events.Payments;

public sealed record PaymentSucceededEvent : DomainEvent
{
    public required Guid OrderId { get; init; }

    public required Money Amount { get; init; }

    public required string Reference { get; init; }
}
