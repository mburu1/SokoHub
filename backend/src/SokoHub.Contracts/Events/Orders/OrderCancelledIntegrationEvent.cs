using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Orders;

public record OrderCancelledIntegrationEvent(
    Guid OrderId,
    string OrderNumber,
    Guid CustomerId,
    string Reason) : IntegrationEvent;
