namespace SokoHub.Domain.Events.Vendors;

public sealed record VendorApprovedEvent : DomainEvent
{
    public required string DisplayName { get; init; }
}
