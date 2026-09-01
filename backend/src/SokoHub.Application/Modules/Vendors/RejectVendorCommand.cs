using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Vendors;

public record RejectVendorCommand(
    Guid VendorId,
    string Reason,
    string RejectedBy) : IRequest<bool>;

public sealed class RejectVendorHandler : IRequestHandler<RejectVendorCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RejectVendorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RejectVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor == null)
        {
            throw new KeyNotFoundException("Vendor not found.");
        }

        vendor.Reject(request.Reason);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
