namespace SokoHub.Application.Modules.Payments.Sagas;

public sealed class PaymentSaga : SokoHub.Application.Common.Sagas.ISaga
{
    public Guid CorrelationId { get; init; }
}
