namespace SokoHub.Domain.Events.Vendors;

public sealed record VendorRejectedEvent : DomainEvent
{
    public required string Reason { get; init; }
}
