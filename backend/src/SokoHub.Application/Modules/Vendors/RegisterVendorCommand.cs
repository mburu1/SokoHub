using MediatR;
using SokoHub.Contracts.Vendors;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Vendors;

public record RegisterVendorCommand(
    Guid UserId,
    string BusinessName,
    string TaxId,
    decimal CommissionRate) : IRequest<VendorResponse>;

public sealed class RegisterVendorHandler : IRequestHandler<RegisterVendorCommand, VendorResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public RegisterVendorHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<VendorResponse> Handle(RegisterVendorCommand request, CancellationToken cancellationToken)
    {
        // Ensure user is registering for themselves or is an admin
        if (request.UserId != _currentUser.Id)
        {
            throw new UnauthorizedAccessException("You can only register a vendor account for yourself.");
        }

        var taxId = KraPin.Create(request.TaxId);
        var commissionRate = Percentage.Create(request.CommissionRate);

        var vendor = Vendor.Register(
            request.UserId,
            request.BusinessName,
            taxId,
            commissionRate);

        await _unitOfWork.Repository<Vendor>().AddAsync(vendor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new VendorResponse(
            vendor.Id,
            vendor.UserId,
            vendor.BusinessName,
            vendor.TaxId.Value,
            vendor.CommissionRate.Value,
            vendor.Status.ToString());
    }
}
