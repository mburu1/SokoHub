using SokoHub.Domain.Common;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Customers;

public sealed class Customer : AggregateRoot
{
    private readonly List<CustomerAddress> _addresses = [];
    private readonly List<CustomerPreference> _preferences = [];

    private Customer()
    {
    }

    private Customer(Guid id, Guid userId, EmailAddress email, PhoneNumber phone)
        : base(id)
    {
        UserId = userId;
        Email = email;
        Phone = phone;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    public Guid UserId { get; private set; } = null!;
    public EmailAddress Email { get; private set; } = null!;
    public PhoneNumber Phone { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyList<CustomerAddress> Addresses => _addresses.AsReadOnly();
    public IReadOnlyList<CustomerPreference> Preferences => _preferences.AsReadOnly();

    public static Customer Create(Guid userId, EmailAddress email, PhoneNumber phone, Guid? id = null) =>
        new(
            id ?? Guid.Empty,
            Ensure.NotEmpty(userId),
            email,
            phone);

    public void AddAddress(CustomerAddress address)
    {
        _addresses.Add(address);
        Touch();
    }

    public void RemoveAddress(Guid addressId)
    {
        _addresses.RemoveAll(a => a.Id == addressId);
        Touch();
    }

    public void UpdatePreferences(CustomerPreference preference)
    {
        var existing = _preferences.FirstOrDefault(p => p.Key == preference.Key);
        if (existing != null)
        {
            _preferences.Remove(existing);
        }
        _preferences.Add(preference);
        Touch();
    }
}
