namespace SokoHub.Contracts.Auth;

public record RefreshTokenResponse(
    string AccessToken,
    string RefreshToken);
