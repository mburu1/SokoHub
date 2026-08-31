using SokoHub.Domain.Common.Entity;

namespace SokoHub.Domain.Modules.Identity;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public string? RevokedBy { get; private set; }

    private RefreshToken() { }

    private RefreshToken(Guid userId, string tokenHash, DateTimeOffset expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        IsActive = true;
    }

    public static RefreshToken Issue(Guid userId, string tokenHash, DateTimeOffset expiresAt)
    {
        return new RefreshToken(userId, tokenHash, expiresAt);
    }

    public void Revoke(string reason)
    {
        IsActive = false;
        RevokedAt = DateTimeOffset.UtcNow;
        RevokedBy = reason;
    }

    public bool IsExpired => DateTimeOffset.UtcNow >= ExpiresAt;
}
