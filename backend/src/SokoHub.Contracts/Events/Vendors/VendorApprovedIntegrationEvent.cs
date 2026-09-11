using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Vendors;

public record VendorApprovedIntegrationEvent(
    Guid VendorId,
    string StoreName,
    decimal CommissionRate) : IntegrationEvent;
