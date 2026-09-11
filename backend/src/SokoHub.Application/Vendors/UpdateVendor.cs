using MediatR;
using SokoHub.Contracts.Vendors;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;

namespace SokoHub.Application.Vendors;

public record UpdateVendorCommand(
    Guid VendorId,
    string BusinessName,
    decimal CommissionRate) : IRequest<VendorResponse>;

public sealed class UpdateVendorHandler : IRequestHandler<UpdateVendorCommand, VendorResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVendorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VendorResponse> Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor == null)
        {
            throw new KeyNotFoundException($"Vendor with ID {request.VendorId} was not found.");
        }

        vendor.UpdateProfile(request.BusinessName);
        vendor.UpdateCommission(Percentage.Create(request.CommissionRate));

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
