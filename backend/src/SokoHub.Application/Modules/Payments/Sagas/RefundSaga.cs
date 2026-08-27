namespace SokoHub.Application.Modules.Payments.Sagas;

public sealed class RefundSaga : SokoHub.Application.Common.Sagas.ISaga
{
    public Guid CorrelationId { get; init; }
}
