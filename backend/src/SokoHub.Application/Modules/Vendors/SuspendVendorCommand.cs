using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Vendors;

public record SuspendVendorCommand(
    Guid VendorId,
    string Reason) : IRequest<Result>;

public sealed class SuspendVendorHandler : IRequestHandler<SuspendVendorCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public SuspendVendorHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(SuspendVendorCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUser.IsAdmin)
        {
            return Result.Failure(new ApplicationError("unauthorized", "Only administrators can suspend vendors."));
        }

        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);

        if (vendor == null)
        {
            return Result.Failure(new ApplicationError("vendor_not_found", "Vendor not found."));
        }

        try
        {
            vendor.Suspend(request.Reason);
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("suspension_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
