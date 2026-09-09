using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SokoHub.Infrastructure.Payments.Mpesa.Daraja;

public class DarajaAuthenticationClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly ILogger<DarajaAuthenticationClient> _logger;
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public DarajaAuthenticationClient(HttpClient httpClient, IConfiguration config, ILogger<DarajaAuthenticationClient> logger)
    {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry)
        {
            return _cachedToken;
        }

        var consumerKey = _config["Mpesa:ConsumerKey"];
        var consumerSecret = _config["Mpesa:ConsumerSecret"];

        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{consumerKey}:{consumerSecret}"));

        var request = new HttpRequestMessage(HttpMethod.Get, $"{_config["Mpesa:DarajaUrl"]}/oauth/v1/generate?grant_type=client_credentials");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Failed to acquire M-Pesa Access Token. Status: {Status}", response.StatusCode);
            throw new Exception("M-Pesa Authentication failed.");
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var data = JsonSerializer.Deserialize<MpesaAuthResponse>(content);

        _cachedToken = data?.AccessToken;
        _tokenExpiry = DateTime.UtcNow.AddSeconds(data?.ExpiresIn ?? 3600) - TimeSpan.FromMinutes(5);

        return _cachedToken!;
    }

    private record MpesaAuthResponse(string AccessToken, int ExpiresIn);
}
