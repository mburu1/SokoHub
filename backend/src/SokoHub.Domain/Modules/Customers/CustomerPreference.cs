using SokoHub.Domain.Common;

namespace SokoHub.Domain.Modules.Customers;

public sealed class CustomerPreference : Entity
{
    private CustomerPreference()
    {
    }

    public CustomerPreference(Guid id, Guid customerId, string key, string value)
        : base(id)
    {
        CustomerId = customerId;
        Key = Ensure.NotBlank(key);
        Value = value;
    }

    public Guid CustomerId { get; private set; } = null!;
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
}
