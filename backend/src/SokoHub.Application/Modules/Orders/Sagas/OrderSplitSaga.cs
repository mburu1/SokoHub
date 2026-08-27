namespace SokoHub.Application.Modules.Orders.Sagas;

/// <summary>One cart → N vendor sub-orders.</summary>
public sealed class OrderSplitSaga : SokoHub.Application.Common.Sagas.ISaga
{
    public Guid CorrelationId { get; init; }
}
