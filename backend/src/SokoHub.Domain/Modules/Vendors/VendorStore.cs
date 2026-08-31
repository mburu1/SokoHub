using SokoHub.Domain.Common;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Domain.Modules.Vendors;

public sealed class VendorStore : Entity
{
    private VendorStore()
    {
    }

    public VendorStore(Guid id, Guid vendorId, string storeName, string description, string logoUrl, string bannerUrl)
        : base(id)
    {
        VendorId = vendorId;
        StoreName = Ensure.MaxLength(Ensure.NotBlank(storeName), 100);
        Description = Ensure.MaxLength(description, 1000);
        LogoUrl = logoUrl;
        BannerUrl = bannerUrl;
        IsActive = true;
    }

    public Guid VendorId { get; private set; } = null!;
    public string StoreName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string LogoUrl { get; private set; } = string.Empty;
    public string BannerUrl { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void UpdateProfile(string name, string description, string logoUrl, string bannerUrl)
    {
        StoreName = Ensure.MaxLength(Ensure.NotBlank(name), 100);
        Description = Ensure.MaxLength(description, 1000);
        LogoUrl = logoUrl;
        BannerUrl = bannerUrl;
    }
}
