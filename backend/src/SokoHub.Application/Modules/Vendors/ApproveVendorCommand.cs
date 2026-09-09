using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Vendors;

public record ApproveVendorCommand(
    Guid VendorId,
    string ApprovedBy) : IRequest<Result>;

public sealed class ApproveVendorHandler : IRequestHandler<ApproveVendorCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ApproveVendorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ApproveVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor == null)
        {
            return Result.Failure(new ApplicationError("vendor_not_found", "Vendor not found."));
        }

        try
        {
            vendor.VerifyKyc(Guid.Empty, request.ApprovedBy); // simplified for now
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("approval_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
