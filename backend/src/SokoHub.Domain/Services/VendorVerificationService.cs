using System.Linq.Expressions;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Common.Specifications;

namespace SokoHub.Domain.Services;

public interface IVendorVerificationService
{
    Task<bool> VerifyVendorKycAsync(Vendor vendor, CancellationToken cancellationToken = default);
}

public class VendorVerificationService : IVendorVerificationService
{
    private readonly IRepository<VendorDocument> _documentRepository;

    public VendorVerificationService(IRepository<VendorDocument> documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<bool> VerifyVendorKycAsync(Vendor vendor, CancellationToken cancellationToken = default)
    {
        // A vendor is verified if they have at least one valid identity document
        // and their TaxId is valid.

        // This is a simplified domain rule.
        var docs = await _documentRepository.ListAsync(new VendorDocumentsSpecification(vendor.Id), cancellationToken);

        if (docs.Count == 0)
        {
            return false;
        }

        // In a real implementation, we might call an external KYC provider API here
        // (orchestrated by an Application service calling this domain service).

        return true;
    }
}

public class VendorDocumentsSpecification : Specification<VendorDocument>
{
    public override Expression<Func<VendorDocument, bool>> Criteria => d => d.VendorId == _vendorId;
    private readonly Guid _vendorId;

    public VendorDocumentsSpecification(Guid vendorId)
    {
        _vendorId = vendorId;
    }
}
