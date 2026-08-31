namespace SokoHub.Domain.Events.Customers;

public sealed record CustomerRegisteredEvent : DomainEvent
{
    public required string Email { get; init; }

    public required string Phone { get; init; }
}
