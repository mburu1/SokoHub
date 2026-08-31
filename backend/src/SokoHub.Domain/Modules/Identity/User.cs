namespace SokoHub.Domain.Modules.Identity;

public sealed class User : AggregateRoot
{
    private readonly List<Guid> _roleIds = [];
    private readonly List<RefreshToken> _refreshTokens = [];
    private readonly List<UserSession> _sessions = [];
    private readonly List<OAuthAccount> _oauthAccounts = [];

    private User()
    {
    }

    private User(Guid id, EmailAddress email, PhoneNumber phone, string displayName, string passwordHash)
        : base(id)
    {
        Email = email;
        Phone = phone;
        DisplayName = displayName;
        PasswordHash = passwordHash;
        Status = UserStatus.PendingVerification;
        SecurityStamp = Guid.CreateVersion7().ToString("N");
    }

    public EmailAddress Email { get; private set; } = null!;

    public PhoneNumber Phone { get; private set; } = null!;

    public string DisplayName { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public string SecurityStamp { get; private set; } = string.Empty;

    public UserStatus Status { get; private set; }

    public int FailedAccessCount { get; private set; }

    public DateTimeOffset? LockoutEnd { get; private set; }

    public IReadOnlyList<Guid> RoleIds => _roleIds.AsReadOnly();

    public IReadOnlyList<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    public IReadOnlyList<UserSession> Sessions => _sessions.AsReadOnly();

    public IReadOnlyList<OAuthAccount> OAuthAccounts => _oauthAccounts.AsReadOnly();

    public static User Register(EmailAddress email, PhoneNumber phone, string displayName, string passwordHash, Guid? id = null) =>
        new(
            id ?? Guid.Empty,
            email,
            phone,
            Ensure.MaxLength(Ensure.NotBlank(displayName), 120),
            Ensure.NotBlank(passwordHash));

    public void Verify()
    {
        Ensure.That(Status == UserStatus.PendingVerification, "user_not_pending", "User is not pending verification.");
        Status = UserStatus.Active;
        IncrementVersion();
    }

    public void AssignRole(Guid roleId)
    {
        Ensure.NotEmpty(roleId);
        if (!_roleIds.Contains(roleId))
        {
            _roleIds.Add(roleId);
            Touch();
        }
    }

    public void RemoveRole(Guid roleId)
    {
        _roleIds.Remove(roleId);
        Touch();
    }

    public void ChangePassword(string passwordHash)
    {
        PasswordHash = Ensure.NotBlank(passwordHash);
        RotateSecurityStamp();
        RevokeAllRefreshTokens("password_changed");
        IncrementVersion();
    }

    public void RecordFailedAccess(int maxAttempts, TimeSpan lockout)
    {
        FailedAccessCount++;
        if (FailedAccessCount >= maxAttempts)
        {
            Status = UserStatus.Locked;
            LockoutEnd = DateTimeOffset.UtcNow.Add(lockout);
        }

        Touch();
    }

    public void RecordSuccessfulAccess()
    {
        FailedAccessCount = 0;
        if (Status == UserStatus.Locked && LockoutEnd <= DateTimeOffset.UtcNow)
        {
            Status = UserStatus.Active;
            LockoutEnd = null;
        }

        Touch();
    }

    public RefreshToken IssueRefreshToken(string tokenHash, DateTimeOffset expiresAt)
    {
        Ensure.That(Status == UserStatus.Active || Status == UserStatus.PendingVerification, "user_not_active", "Inactive users cannot receive refresh tokens.");
        var token = RefreshToken.Issue(Id, tokenHash, expiresAt);
        _refreshTokens.Add(token);
        Touch();
        return token;
    }

    public UserSession OpenSession(string ipAddress, string userAgent)
    {
        var session = UserSession.Open(Id, ipAddress, userAgent);
        _sessions.Add(session);
        Touch();
        return session;
    }

    public OAuthAccount LinkOAuth(string provider, string providerUserId)
    {
        Ensure.That(_oauthAccounts.TrueForAll(a => a.Provider != provider), "oauth_duplicate", $"Provider '{provider}' is already linked.");
        var account = OAuthAccount.Link(Id, provider, providerUserId);
        _oauthAccounts.Add(account);
        Touch();
        return account;
    }

    public void Disable(string reason)
    {
        Ensure.NotBlank(reason);
        Status = UserStatus.Disabled;
        RevokeAllRefreshTokens(reason);
        IncrementVersion();
    }

    private void RotateSecurityStamp() => SecurityStamp = Guid.CreateVersion7().ToString("N");

    private void RevokeAllRefreshTokens(string reason)
    {
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
        {
            token.Revoke(reason);
        }
    }
}
