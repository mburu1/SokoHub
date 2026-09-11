using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Infrastructure.Common;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    private static readonly Dictionary<string, Guid> RoleMap = new()
    {
        ["Admin"] = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        ["Vendor"] = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        ["Customer"] = Guid.Parse("33333333-3333-3333-3333-333333333333"),
    };

    public Guid? Id
    {
        get
        {
            var userId = User?.FindFirst("sub")?.Value
                        ?? User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value ?? User?.FindFirst("email")?.Value;

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;

    public IReadOnlyList<Guid> RoleIds
    {
        get
        {
            var role = Role;
            if (string.IsNullOrEmpty(role) || !RoleMap.TryGetValue(role, out var roleId))
            {
                return Array.Empty<Guid>();
            }

            return new[] { roleId };
        }
    }

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin => HasRoleClaim("Admin");

    public bool IsVendor => HasRoleClaim("Vendor");

    public bool IsCustomer => HasRoleClaim("Customer");

    private bool HasRoleClaim(string role) =>
        User?.IsInRole(role) == true ||
        string.Equals(Role, role, StringComparison.OrdinalIgnoreCase);
}
