namespace SokoHub.Application.Common.Interfaces;

public interface IJwtProvider
{
    (string AccessToken, DateTime Expiration) GenerateToken(User user);

    string GenerateRefreshToken();
}