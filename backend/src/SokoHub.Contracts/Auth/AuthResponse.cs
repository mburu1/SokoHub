namespace SokoHub.Contracts.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime[] Expiration,
    Guid UserId,
    string Email);
