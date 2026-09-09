using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Vendors;

public record SubmitKycCommand(
    Guid VendorId,
    string DocumentType,
    string DocumentUrl,
    string Checksum) : IRequest<Result>;

public sealed class SubmitKycHandler : IRequestHandler<SubmitKycCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public SubmitKycHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(SubmitKycCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor == null)
        {
            return Result.Failure(new ApplicationError("vendor_not_found", "Vendor not found."));
        }

        if (vendor.UserId != _currentUser.Id)
        {
            return Result.Failure(new ApplicationError("unauthorized", "You can only submit KYC for your own vendor account."));
        }

        var document = new VendorDocument(
            Guid.NewGuid(),
            vendor.Id,
            request.DocumentType,
            request.DocumentUrl,
            request.Checksum);

        vendor.AddDocument(document);

        await _unitOfWork.Repository<VendorDocument>().AddAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
