using MediatR;
using SokoHub.Contracts.Vendors;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Domain.Common.Specifications;

namespace SokoHub.Application.Modules.Vendors.Queries;

public record GetByIdQuery(Guid Id) : IRequest<VendorResponse>;

public sealed class GetByIdHandler : IRequestHandler<GetByIdQuery, VendorResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<VendorResponse> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.Id, cancellationToken);

        if (vendor == null)
        {
            throw new KeyNotFoundException("Vendor not found.");
        }

        return new VendorResponse(
            vendor.Id,
            vendor.UserId,
            vendor.BusinessName,
            vendor.TaxId.Value,
            vendor.CommissionRate.Value,
            vendor.Status.ToString());
    }
}
