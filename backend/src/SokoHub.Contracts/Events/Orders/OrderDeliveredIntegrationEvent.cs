using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Orders;

public record OrderDeliveredIntegrationEvent(
    Guid OrderId,
    string OrderNumber,
    Guid CustomerId,
    DateTimeOffset DeliveredAt) : IntegrationEvent;
