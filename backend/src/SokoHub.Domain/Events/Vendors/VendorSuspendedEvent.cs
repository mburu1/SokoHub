namespace SokoHub.Domain.Events.Vendors;

public sealed record VendorSuspendedEvent : DomainEvent
{
    public required string Reason { get; init; }
}
