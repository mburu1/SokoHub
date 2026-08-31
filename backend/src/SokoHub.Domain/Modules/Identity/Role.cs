namespace SokoHub.Domain.Modules.Identity;

public sealed class Role : AggregateRoot
{
    private readonly List<Guid> _permissionIds = [];

    private Role()
    {
    }

    private Role(Guid id, string name, string normalizedName)
        : base(id)
    {
        Name = name;
        NormalizedName = normalizedName;
    }

    public string Name { get; private set; } = string.Empty;

    public string NormalizedName { get; private set; } = string.Empty;

    public IReadOnlyList<Guid> PermissionIds => _permissionIds.AsReadOnly();

    public static Role Create(string name, Guid? id = null)
    {
        var trimmed = Ensure.MaxLength(Ensure.NotBlank(name), 64);
        return new Role(id ?? Guid.Empty, trimmed, trimmed.ToUpperInvariant());
    }

    public void Grant(Guid permissionId)
    {
        if (!_permissionIds.Contains(permissionId))
        {
            _permissionIds.Add(Ensure.NotEmpty(permissionId));
            Touch();
        }
    }

    public void Revoke(Guid permissionId)
    {
        _permissionIds.Remove(permissionId);
        Touch();
    }
}
