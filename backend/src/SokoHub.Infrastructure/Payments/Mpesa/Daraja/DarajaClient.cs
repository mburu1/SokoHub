using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SokoHub.Infrastructure.Payments.Mpesa.Daraja;

public class DarajaClient
{
    private readonly HttpClient _httpClient;
    private readonly DarajaAuthenticationClient _authClient;
    private readonly DarajaRequestBuilder _requestBuilder;
    private readonly DarajaResponseMapper _responseMapper;
    private readonly IConfiguration _config;
    private readonly ILogger<DarajaClient> _logger;

    public DarajaClient(
        HttpClient httpClient,
        DarajaAuthenticationClient authClient,
        DarajaRequestBuilder requestBuilder,
        DarajaResponseMapper responseMapper,
        IConfiguration config,
        ILogger<DarajaClient> logger)
    {
        _httpClient = httpClient;
        _authClient = authClient;
        _requestBuilder = requestBuilder;
        _responseMapper = responseMapper;
        _config = config;
        _logger = logger;
    }

    public async Task<string> InitiateStkPushAsync(string phoneNumber, decimal amount, string reference, string description, CancellationToken cancellationToken = default)
    {
        var token = await _authClient.GetAccessTokenAsync(cancellationToken);
        var payload = _requestBuilder.BuildStkPushRequest(
            _config["Mpesa:ShortCode"],
            _config["Mpesa:Passphrase"],
            phoneNumber,
            amount,
            reference,
            description);

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_config["Mpesa:DarajaUrl"]}/mpesa/stkpush/v1/processrequest");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("M-Pesa STK Push failed: {Content}", content);
            throw new Exception($"M-Pesa API Error: {response.StatusCode}");
        }

        var mapped = _responseMapper.MapStkPushResponse(content);
        return mapped.CheckoutRequestID;
    }

    public async Task<bool> QueryTransactionAsync(string checkoutRequestId, CancellationToken cancellationToken = default)
    {
        var token = await _authClient.GetAccessTokenAsync(cancellationToken);
        var request = new HttpRequestMessage(HttpMethod.Get, $"{_config["Mpesa:DarajaUrl"]}/mpesa/stkpush/v1/query?CheckoutRequestID={checkoutRequestId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
