using MediatR;
using SokoHub.Contracts.Vendors;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Vendors;

public record RegisterVendorCommand(
    Guid UserId,
    string BusinessName,
    string TaxId,
    decimal CommissionRate) : IRequest<Result<VendorResponse>>;

public sealed class RegisterVendorHandler : IRequestHandler<RegisterVendorCommand, Result<VendorResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public RegisterVendorHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<VendorResponse>> Handle(RegisterVendorCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId != _currentUser.Id)
        {
            return Result<VendorResponse>.Failure(new ApplicationError("unauthorized", "You can only register a vendor account for yourself."));
        }

        try
        {
            var taxId = KraPin.Create(request.TaxId);
            var commissionRate = Percentage.Create(request.CommissionRate);

            var vendor = Vendor.Register(
                request.UserId,
                request.BusinessName,
                taxId,
                commissionRate);

            await _unitOfWork.Repository<Vendor>().AddAsync(vendor, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<VendorResponse>.Success(new VendorResponse(
                vendor.Id,
                vendor.UserId,
                vendor.BusinessName,
                vendor.TaxId.Value,
                vendor.CommissionRate.Value,
                vendor.Status.ToString()));
        }
        catch (Exception ex)
        {
            return Result<VendorResponse>.Failure(new ApplicationError("registration_failed", ex.Message));
        }
    }
}
