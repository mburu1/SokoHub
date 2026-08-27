namespace SokoHub.Application.Modules.Checkout.Sagas;

/// <summary>
/// Coordinates: inventory reservation → payment initiation → order creation / split.
/// Compensating actions on failure (release stock, cancel pending payment).
/// </summary>
public sealed class CheckoutSaga : SokoHub.Application.Common.Sagas.ISaga
{
    public Guid CorrelationId { get; init; }
    // TODO: state machine + handlers
}
