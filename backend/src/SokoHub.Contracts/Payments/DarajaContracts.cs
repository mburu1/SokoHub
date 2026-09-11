using System.Text.Json.Serialization;

namespace SokoHub.Contracts.Payments;

public record DarajaStkPushRequest(
    string BusinessShortCode,
    string Password,
    string Timestamp,
    string TransactionType,
    decimal Amount,
    string PartyA,
    string PartyB,
    string PhoneNumber,
    string CallBackURL,
    string AccountReference,
    string TransactionDesc);

public record DarajaStkPushResponse(
    [property: JsonPropertyName("MerchantRequestID")] string MerchantRequestId,
    [property: JsonPropertyName("CheckoutRequestID")] string CheckoutRequestId,
    [property: JsonPropertyName("ResponseCode")] string ResponseCode,
    [property: JsonPropertyName("ResponseDescription")] string ResponseDescription,
    [property: JsonPropertyName("CustomerMessage")] string CustomerMessage);

public record DarajaCallbackBody(
    [property: JsonPropertyName("stkCallback")] DarajaStkCallback StkCallback);

public record DarajaStkCallback(
    [property: JsonPropertyName("MerchantRequestID")] string MerchantRequestId,
    [property: JsonPropertyName("CheckoutRequestID")] string CheckoutRequestId,
    [property: JsonPropertyName("ResultCode")] int ResultCode,
    [property: JsonPropertyName("ResultDesc")] string ResultDesc,
    [property: JsonPropertyName("CallbackMetadata")] DarajaCallbackMetadata? CallbackMetadata);

public record DarajaCallbackMetadata(
    [property: JsonPropertyName("Item")] List<DarajaCallbackItem> Item);

public record DarajaCallbackItem(
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("Value")] object? Value);

public record DarajaCallbackPayload(
    [property: JsonPropertyName("Body")] DarajaCallbackBody Body);

public record DarajaB2CRequest(
    string InitiatorName,
    string SecurityCredential,
    string CommandID,
    decimal Amount,
    string PartyA,
    string PartyB,
    string Remarks,
    string QueueTimeOutURL,
    string ResultURL,
    string Occasion);

public record DarajaB2CResponse(
    [property: JsonPropertyName("ConversationID")] string ConversationId,
    [property: JsonPropertyName("OriginatorConversationID")] string OriginatorConversationId,
    [property: JsonPropertyName("ResponseCode")] string ResponseCode,
    [property: JsonPropertyName("ResponseDescription")] string ResponseDescription);
