using MediatR;
using SokoHub.Contracts.Vendors;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Vendors;

namespace SokoHub.Application.Vendors;

public record RegisterVendorCommand(
    Guid UserId,
    string BusinessName,
    string TaxId,
    decimal CommissionRate) : IRequest<VendorResponse>;

public sealed class RegisterVendorHandler : IRequestHandler<RegisterVendorCommand, VendorResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterVendorHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VendorResponse> Handle(RegisterVendorCommand request, CancellationToken cancellationToken)
    {
        var taxId = KraPin.Create(request.TaxId);
        var commission = Percentage.Create(request.CommissionRate);

        var vendor = Vendor.Register(request.UserId, request.BusinessName, taxId, commission);

        // await _unitOfWork.Repository<Vendor>().AddAsync(vendor);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new VendorResponse(
            vendor.Id,
            vendor.UserId,
            vendor.BusinessName,
            vendor.TaxId.Value,
            vendor.CommissionRate.Value,
            vendor.Status.ToString());
    }
}
