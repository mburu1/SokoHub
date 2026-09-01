using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Vendors;

public record SubmitKycCommand(
    Guid VendorId,
    string DocumentType,
    string DocumentUrl,
    string Checksum) : IRequest<bool>;

public sealed class SubmitKycHandler : IRequestHandler<SubmitKycCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public SubmitKycHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(SubmitKycCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor == null)
        {
            throw new KeyNotFoundException("Vendor not found.");
        }

        if (vendor.UserId != _currentUser.Id)
        {
            throw new UnauthorizedAccessException("You can only submit KYC for your own vendor account.");
        }

        var document = new VendorDocument(
            Guid.NewGuid(),
            vendor.Id,
            request.DocumentType,
            request.DocumentUrl,
            request.Checksum);

        vendor.AddDocument(document);

        // In a real scenario, we might change status to UnderReview here.
        // Let's assume the domain entity has a method for this.
        // Actually, let's add a method to Vendor to set status to UnderReview when first document is uploaded.

        await _unitOfWork.Repository<VendorDocument>().AddAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
