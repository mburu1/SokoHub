using SokoHub.Contracts.IntegrationEvents;

namespace SokoHub.Contracts.Events.Orders;

public record OrderConfirmedIntegrationEvent(
    Guid OrderId,
    string OrderNumber,
    Guid CustomerId,
    decimal TotalAmount,
    string PaymentReference) : IntegrationEvent;
