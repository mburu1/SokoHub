using MediatR;
using SokoHub.Contracts.Vendors;
using SokoHub.Domain.Common.ValueObjects;
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
        // var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId);
        // if (vendor == null) throw new NotFoundException("Vendor not found");

        // vendor.UpdateCommission(Percentage.Create(request.CommissionRate));
        // vendor.UpdateProfile(request.BusinessName); // Note: Need to add UpdateProfile to Vendor entity

        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        throw new NotImplementedException("UpdateVendor requires repository connectivity.");
    }
}
