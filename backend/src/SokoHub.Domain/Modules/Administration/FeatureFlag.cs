using SokoHub.Domain.Common.AggregateRoots;

namespace SokoHub.Domain.Modules.Administration;

public class FeatureFlag : AggregateRoot
{
    public string Key { get; private set; } = null!;
    public bool IsEnabled { get; private set; }
    public string Description { get; private set; } = null!;

    private FeatureFlag() { }

    public FeatureFlag(Guid id, string key, bool isEnabled, string description)
        : base(id)
    {
        Key = key;
        IsEnabled = isEnabled;
        Description = description;
        Touch();
    }

    public void Toggle()
    {
        IsEnabled = !IsEnabled;
        Touch();
    }

    public void UpdateDescription(string description)
    {
        Description = description;
        Touch();
    }
}
