using MediatR;
using SokoHub.Domain.Modules.Vendors;

namespace SokoHub.Application.Vendors;

public record VerifyVendorCommand(
    Guid VendorId,
    Guid DocumentId,
    string VerifiedBy) : IRequest<bool>;

public sealed class VerifyVendorHandler : IRequestHandler<VerifyVendorCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public VerifyVendorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(VerifyVendorCommand request, CancellationToken cancellationToken)
    {
        // var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId);
        // if (vendor == null) throw new NotFoundException("Vendor not found");

        // vendor.VerifyKyc(request.DocumentId, request.VerifiedBy);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
