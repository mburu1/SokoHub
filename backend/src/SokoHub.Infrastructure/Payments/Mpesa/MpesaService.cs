using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Contracts.Payments;
using SokoHub.Infrastructure.Payments.Mpesa.Daraja;

namespace SokoHub.Infrastructure.Payments.Mpesa;

public class MpesaService : IMpesaService
{
    private readonly HttpClient _httpClient;
    private readonly DarajaAuthenticationClient _authClient;
    private readonly IConfiguration _config;
    private readonly ILogger<MpesaService> _logger;

    public MpesaService(
        HttpClient httpClient,
        DarajaAuthenticationClient authClient,
        IConfiguration config,
        ILogger<MpesaService> logger)
    {
        _httpClient = httpClient;
        _authClient = authClient;
        _config = config;
        _logger = logger;
    }

    public async Task<DarajaStkPushResponse> InitiateStkPushAsync(
        string phoneNumber,
        decimal amount,
        string accountReference,
        string transactionDesc,
        CancellationToken cancellationToken = default)
    {
        var token = await _authClient.GetAccessTokenAsync(cancellationToken);
        var shortCode = _config["Mpesa:ShortCode"] ?? "174379";
        var passkey = _config["Mpesa:Passkey"] ?? "bfb279f9aa9bdbcf158e97dd71a467cd2e0c893059b10f78e6b72ada1ed2c919";
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var password = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{shortCode}{passkey}{timestamp}"));
        var callbackUrl = _config["Mpesa:CallbackUrl"] ?? "https://api.sokohub.co.ke/api/payments/mpesa/callback";

        var payload = new
        {
            BusinessShortCode = shortCode,
            Password = password,
            Timestamp = timestamp,
            TransactionType = "CustomerPayBillOnline",
            Amount = (int)amount,
            PartyA = phoneNumber,
            PartyB = shortCode,
            PhoneNumber = phoneNumber,
            CallBackURL = callbackUrl,
            AccountReference = accountReference,
            TransactionDesc = transactionDesc
        };

        var darajaUrl = _config["Mpesa:DarajaUrl"] ?? "https://sandbox.safaricom.co.ke";
        var request = new HttpRequestMessage(HttpMethod.Post, $"{darajaUrl}/mpesa/stkpush/v1/processrequest");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("M-Pesa STK Push error: {Content}", content);
            throw new InvalidOperationException($"Daraja STK push failed with status {response.StatusCode}: {content}");
        }

        var result = JsonSerializer.Deserialize<DarajaStkPushResponse>(content);
        return result ?? throw new InvalidOperationException("Failed to deserialize Daraja STK Push response.");
    }

    public async Task<DarajaB2CResponse> InitiateB2CPayoutAsync(
        string phoneNumber,
        decimal amount,
        string remarks,
        string occasion,
        CancellationToken cancellationToken = default)
    {
        var token = await _authClient.GetAccessTokenAsync(cancellationToken);
        var shortCode = _config["Mpesa:B2CShortCode"] ?? "600000";
        var initiator = _config["Mpesa:B2CInitiatorName"] ?? "testapi";
        var credential = _config["Mpesa:B2CSecurityCredential"] ?? "credential";
        var queueTimeoutUrl = _config["Mpesa:B2CTimeoutUrl"] ?? "https://api.sokohub.co.ke/api/payments/mpesa/b2c/timeout";
        var resultUrl = _config["Mpesa:B2CResultUrl"] ?? "https://api.sokohub.co.ke/api/payments/mpesa/b2c/result";

        var payload = new
        {
            InitiatorName = initiator,
            SecurityCredential = credential,
            CommandID = "BusinessPayment",
            Amount = (int)amount,
            PartyA = shortCode,
            PartyB = phoneNumber,
            Remarks = remarks,
            QueueTimeOutURL = queueTimeoutUrl,
            ResultURL = resultUrl,
            Occasion = occasion
        };

        var darajaUrl = _config["Mpesa:DarajaUrl"] ?? "https://sandbox.safaricom.co.ke";
        var request = new HttpRequestMessage(HttpMethod.Post, $"{darajaUrl}/mpesa/b2c/v1/paymentrequest");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("M-Pesa B2C error: {Content}", content);
            throw new InvalidOperationException($"Daraja B2C failed with status {response.StatusCode}: {content}");
        }

        var result = JsonSerializer.Deserialize<DarajaB2CResponse>(content);
        return result ?? throw new InvalidOperationException("Failed to deserialize Daraja B2C response.");
    }
}
