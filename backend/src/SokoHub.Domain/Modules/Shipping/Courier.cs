using SokoHub.Domain.Common;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Enums;

namespace SokoHub.Domain.Modules.Shipping;

public sealed class Courier : Entity
{
    private Courier() { }

    private Courier(Guid id, string name, string code, string apiKey, string apiSecret)
        : base(id)
    {
        Name = name;
        Code = code;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public string ApiKey { get; private set; } = string.Empty;
    public string ApiSecret { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public static Courier Create(string name, string code, string apiKey, string apiSecret, Guid? id = null)
    {
        return new Courier(id ?? Guid.NewGuid(), name, code, apiKey, apiSecret);
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}