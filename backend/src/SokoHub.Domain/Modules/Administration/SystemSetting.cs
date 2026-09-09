using SokoHub.Domain.Common.AggregateRoots;

namespace SokoHub.Domain.Modules.Administration;

public class SystemSetting : AggregateRoot
{
    public string Key { get; private set; } = null!;
    public string Value { get; private set; } = null!;
    public string Description { get; private set; } = null!;

    private SystemSetting() { }

    public SystemSetting(Guid id, string key, string value, string description)
        : base(id)
    {
        Key = key;
        Value = value;
        Description = description;
        Touch();
    }

    public void UpdateValue(string newValue)
    {
        Value = newValue;
        Touch();
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        Touch();
    }
}
