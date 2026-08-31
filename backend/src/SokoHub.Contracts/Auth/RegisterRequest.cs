namespace SokoHub.Contracts.Auth;

public record RegisterRequest(
    string Email,
    string Phone,
    string DisplayName,
    string Password);
