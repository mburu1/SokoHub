using System.Text.Json;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Infrastructure.Payments.Mpesa.Daraja;

public class DarajaRequestBuilder
{
    public string BuildStkPushRequest(string businessShortCode, string passphrase, string phoneNumber, decimal amount, string accountReference, string transactionDesc)
    {
        var payload = new
        {
            BusinessShortCode = businessShortCode,
            Password = passphrase,
            Timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
            TransactionType = "CustomerPayBillOnline",
            Amount = (int)amount,
            PartyA = phoneNumber,
            PartyB = businessShortCode,
            PhoneNumber = phoneNumber,
            CallBackURL = "https://api.sokohub.com/api/payments/mpesa/callback",
            AccountReference = accountReference,
            TransactionDesc = transactionDesc
        };

        return JsonSerializer.Serialize(payload);
    }

    public string BuildB2CRequest(string shortCode, string amount, string phoneNumber, string accountReference, string transactionDesc)
    {
        var payload = new
        {
            InitiatorName = "SokoHub",
            InitiatorPassword = "Password", // Should be from config
            InitiatorSecurityToken = "Token", // Should be from config
            RecipientPhoneNumber = phoneNumber,
            Amount = amount,
            AccountReference = accountReference,
            TransactionDesc = transactionDesc,
            QueueTimeOutDate = DateTime.UtcNow.AddMinutes(10).ToString("yyyy-MM-ddTHH:mm:ss")
        };

        return JsonSerializer.Serialize(payload);
    }
}
