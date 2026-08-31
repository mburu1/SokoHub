using SokoHub.Domain.Common.Entity;

namespace SokoHub.Domain.Modules.Identity;

public class UserSession : Entity
{
    public Guid UserId { get; private set; }
    public string IpAddress { get; private set; } = null!;
    public string UserAgent { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? LastAccessedAt { get; private set; }
    public bool IsActive { get; private set; }

    private UserSession() { }

    private UserSession(Guid userId, string ipAddress, string userAgent)
    {
        UserId = userId;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAt = DateTimeOffset.UtcNow;
        LastAccessedAt = CreatedAt;
        IsActive = true;
    }

    public static UserSession Open(Guid userId, string ipAddress, string userAgent)
    {
        return new UserSession(userId, ipAddress, userAgent);
    }

    public void Access()
    {
        LastAccessedAt = DateTimeOffset.UtcNow;
    }

    public void Close()
    {
        IsActive = false;
    }
}
