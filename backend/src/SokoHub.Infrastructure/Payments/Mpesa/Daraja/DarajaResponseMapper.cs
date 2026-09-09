using System.Text.Json;

namespace SokoHub.Infrastructure.Payments.Mpesa.Daraja;

public class DarajaResponseMapper
{
    public MpesaStkPushResponse MapStkPushResponse(string json)
    {
        return JsonSerializer.Deserialize<MpesaStkPushResponse>(json) ?? throw new Exception("Failed to map M-Pesa response.");
    }

    public record MpesaStkPushResponse(
        string ResponseDescription,
        string ResponseCode,
        string CheckoutRequestID,
        string CustomerMSISDN);
}
