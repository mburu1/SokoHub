namespace SokoHub.Infrastructure.Payments.Mpesa.StkQuery;

/// <summary>
/// Calls Daraja STK Push Query when callback is missing (~90s).
/// Source of truth for terminal state when webhook is silent.
/// </summary>
public sealed class StkQueryService
{
    // TODO: inject DarajaClient, persist result, publish domain event
}
