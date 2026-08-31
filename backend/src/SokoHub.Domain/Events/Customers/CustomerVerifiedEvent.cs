namespace SokoHub.Domain.Events.Customers;

public sealed record CustomerVerifiedEvent : DomainEvent
{
    public required string Email { get; init; }
}
