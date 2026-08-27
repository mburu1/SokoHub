namespace SokoHub.Api.Middleware.Security;

/// <summary>
/// Restrict M-Pesa callback routes to Safaricom IP ranges (configurable).
/// Pair with fast 200 ACK + queue offload in WebhooksController.
/// </summary>
public sealed class DarajaIpAllowlistMiddleware
{
    // TODO: options + remote IP check
}
