using SokoHub.Contracts.Payments;

namespace SokoHub.Application.Common.Interfaces;

public interface IMpesaService
{
    Task<DarajaStkPushResponse> InitiateStkPushAsync(
        string phoneNumber,
        decimal amount,
        string accountReference,
        string transactionDesc,
        CancellationToken cancellationToken = default);

    Task<DarajaB2CResponse> InitiateB2CPayoutAsync(
        string phoneNumber,
        decimal amount,
        string remarks,
        string occasion,
        CancellationToken cancellationToken = default);
}
