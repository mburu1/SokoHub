namespace SokoHub.Domain.Events.Payments;

public sealed record PaymentFailedEvent : DomainEvent
{
    public required Guid OrderId { get; init; }

    public required string Reason { get; init; }

    public string? ResultCode { get; init; }
}
