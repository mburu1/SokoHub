using SokoHub.Domain.Common;

namespace SokoHub.Domain.Modules.Vendors;

public sealed class VendorDocument : Entity
{
    private VendorDocument()
    {
    }

    public VendorDocument(Guid id, Guid vendorId, string documentType, string documentUrl, string checksum)
        : base(id)
    {
        VendorId = vendorId;
        DocumentType = Ensure.NotBlank(documentType);
        DocumentUrl = Ensure.NotBlank(documentUrl);
        Checksum = checksum;
        UploadedAt = DateTimeOffset.UtcNow;
    }

    public Guid VendorId { get; private set; } = null!;
    public string DocumentType { get; private set; } = string.Empty;
    public string DocumentUrl { get; private set; } = string.Empty;
    public string Checksum { get; private set; } = string.Empty;
    public DateTimeOffset UploadedAt { get; private set; }
}
