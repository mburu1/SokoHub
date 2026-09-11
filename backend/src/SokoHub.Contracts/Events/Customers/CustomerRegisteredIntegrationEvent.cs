using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Customers;

public record CustomerRegisteredIntegrationEvent(
    Guid CustomerId,
    string Email,
    string Phone,
    string DisplayName) : IntegrationEvent;
