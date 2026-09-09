using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Vendors;

public record RejectVendorCommand(
    Guid VendorId,
    string Reason,
    string RejectedBy) : IRequest<Result>;

public sealed class RejectVendorHandler : IRequestHandler<RejectVendorCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RejectVendorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RejectVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor == null)
        {
            return Result.Failure(new ApplicationError("vendor_not_found", "Vendor not found."));
        }

        try
        {
            vendor.Reject(request.Reason);
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("rejection_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
