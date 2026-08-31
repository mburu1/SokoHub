using SokoHub.Domain.Common.Entity;

namespace SokoHub.Domain.Modules.Identity;

public class OAuthAccount : Entity
{
    public Guid UserId { get; private set; }
    public string Provider { get; private set; } = null!;
    public string ProviderUserId { get; private set; } = null!;
    public DateTimeOffset LinkedAt { get; private set; }

    private OAuthAccount() { }

    private OAuthAccount(Guid userId, string provider, string providerUserId)
    {
        UserId = userId;
        Provider = provider;
        ProviderUserId = providerUserId;
        LinkedAt = DateTimeOffset.UtcNow;
    }

    public static OAuthAccount Link(Guid userId, string provider, string providerUserId)
    {
        return new OAuthAccount(userId, provider, providerUserId);
    }

    public void Unlink()
    {
        // In a real system, we might soft-delete or just remove from DB
    }
}
