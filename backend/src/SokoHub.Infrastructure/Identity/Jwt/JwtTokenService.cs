using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Domain.Modules.Identity;

namespace SokoHub.Infrastructure.Identity.Jwt;

public class JwtTokenService : IJwtProvider
{
    private readonly IConfiguration _config;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IConfiguration config, ILogger<JwtTokenService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public (string AccessToken, DateTime Expiration) GenerateToken(User user)
    {
        var secret = _config["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret configuration is missing.");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),
            new(JwtRegisteredClaimNames.UniqueName, user.DisplayName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var roleId in user.RoleIds)
        {
            claims.Add(new Claim(ClaimTypes.Role, roleId.ToString()));
        }

        var expiresInHours = _config.GetValue<int>("Jwt:ExpiresInHours", 2);
        var expires = DateTime.UtcNow.AddHours(expiresInHours);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        _logger.LogDebug("Generated JWT for user {UserId} expiring at {Expires}", user.Id, expires);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
