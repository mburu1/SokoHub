using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Orders;

public record OrderShippedIntegrationEvent(
    Guid OrderId,
    string OrderNumber,
    Guid CustomerId,
    string TrackingNumber,
    string CourierName) : IntegrationEvent;
