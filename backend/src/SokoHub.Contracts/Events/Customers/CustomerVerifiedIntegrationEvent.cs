using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Customers;

public record CustomerVerifiedIntegrationEvent(
    Guid CustomerId,
    string VerificationChannel) : IntegrationEvent;
