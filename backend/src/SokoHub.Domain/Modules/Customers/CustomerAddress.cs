using SokoHub.Domain.Common;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Customers;

public sealed class CustomerAddress : Entity
{
    private CustomerAddress()
    {
    }

    public CustomerAddress(Guid id, Guid customerId, Address address, bool isDefault = false)
        : base(id)
    {
        CustomerId = customerId;
        Address = address;
        IsDefault = isDefault;
    }

    public Guid CustomerId { get; private set; } = null!;
    public Address Address { get; private set; } = null!;
    public bool IsDefault { get; private set; }

    public void SetAsDefault()
    {
        IsDefault = true;
    }

    public void UnsetAsDefault()
    {
        IsDefault = false;
    }
}
