using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Vendors;

public record ApproveVendorCommand(
    Guid VendorId,
    string ApprovedBy) : IRequest<bool>;

public sealed class ApproveVendorHandler : IRequestHandler<ApproveVendorCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveVendorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ApproveVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor == null)
        {
            throw new KeyNotFoundException("Vendor not found.");
        }

        vendor.VerifyKyc(Guid.Empty, request.ApprovedBy); // simplified for now

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
