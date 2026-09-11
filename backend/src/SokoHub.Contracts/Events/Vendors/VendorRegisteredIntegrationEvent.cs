using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Vendors;

public record VendorRegisteredIntegrationEvent(
    Guid VendorId,
    Guid UserId,
    string StoreName,
    string BusinessEmail,
    string BusinessPhone) : IntegrationEvent;
