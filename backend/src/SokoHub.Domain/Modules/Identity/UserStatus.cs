namespace SokoHub.Domain.Modules.Identity;

public enum UserStatus
{
    PendingVerification = 0,
    Active = 1,
    Locked = 2,
    Disabled = 3
}
