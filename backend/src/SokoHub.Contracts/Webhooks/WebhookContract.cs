namespace SokoHub.Contracts.Webhooks;

public record WebhookEvent(
    string Id,
    string Source,
    string EventType,
    string Payload,
    DateTimeOffset ReceivedAtUtc,
    string? Signature = null);

public record WebhookDeliveryResult(
    bool Success,
    int StatusCode,
    string? ResponseBody,
    string? ErrorMessage);
