namespace SokoHub.Domain.Events.Vendors;

public sealed record VendorRegisteredEvent : DomainEvent
{
    public required string DisplayName { get; init; }

    public required string Email { get; init; }
}
