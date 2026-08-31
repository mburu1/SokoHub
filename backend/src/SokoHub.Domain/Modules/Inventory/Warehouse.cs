namespace SokoHub.Domain.Modules.Inventory;

public sealed class Warehouse : AggregateRoot
{
    private readonly List<WarehouseZone> _zones = [];

    private Warehouse()
    {
    }

    private Warehouse(Guid id, Guid? vendorId, string name, Address address)
        : base(id)
    {
        VendorId = vendorId;
        Name = name;
        Address = address;
        IsActive = true;
    }

    public Guid? VendorId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Address Address { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public IReadOnlyList<WarehouseZone> Zones => _zones.AsReadOnly();

    public static Warehouse Create(string name, Address address, Guid? vendorId = null, Guid? id = null) =>
        new(id ?? Guid.Empty, vendorId, Ensure.MaxLength(Ensure.NotBlank(name), 120), address);

    public WarehouseZone AddZone(string code, string name)
    {
        Ensure.That(_zones.TrueForAll(z => !string.Equals(z.Code, code, StringComparison.OrdinalIgnoreCase)), "duplicate_zone", $"Zone '{code}' already exists.");
        var zone = WarehouseZone.Create(Id, code, name);
        _zones.Add(zone);
        Touch();
        return zone;
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
