namespace SokoHub.Contracts.Auth;

public record UserDto(
    Guid Id,
    string Email,
    string Phone,
    string DisplayName,
    IReadOnlyList<string> Roles,
    bool IsActive,
    DateTimeOffset CreatedAt);
