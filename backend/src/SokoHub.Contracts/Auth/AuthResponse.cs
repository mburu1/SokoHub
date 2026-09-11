namespace SokoHub.Contracts.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset Expiration,
    Guid UserId,
    string Email,
    string DisplayName,
    IReadOnlyList<string> Roles);
