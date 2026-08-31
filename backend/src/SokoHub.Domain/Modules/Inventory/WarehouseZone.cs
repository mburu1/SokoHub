namespace SokoHub.Domain.Modules.Inventory;

public sealed class WarehouseZone : Entity
{
    private WarehouseZone()
    {
    }

    private WarehouseZone(Guid id, Guid warehouseId, string code, string name)
        : base(id)
    {
        WarehouseId = warehouseId;
        Code = code;
        Name = name;
    }

    public Guid WarehouseId { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    internal static WarehouseZone Create(Guid warehouseId, string code, string name) =>
        new(Guid.Empty, warehouseId, Ensure.MaxLength(Ensure.NotBlank(code), 16).ToUpperInvariant(), Ensure.MaxLength(Ensure.NotBlank(name), 80));
}
