namespace SokoHub.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid? Id { get; }

    string? Email { get; }

    string? Role { get; }

    IReadOnlyList<Guid> RoleIds { get; }

    bool IsAuthenticated { get; }

    bool IsAdmin { get; }

    bool IsVendor { get; }

    bool IsCustomer { get; }
}
