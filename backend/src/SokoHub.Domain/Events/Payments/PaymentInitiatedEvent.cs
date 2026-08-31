namespace SokoHub.Domain.Events.Payments;

public sealed record PaymentInitiatedEvent : DomainEvent
{
    public required Guid OrderId { get; init; }

    public required Money Amount { get; init; }

    public required string Method { get; init; }

    public string? CheckoutRequestId { get; init; }
}
