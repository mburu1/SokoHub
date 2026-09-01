using MediatR;
using SokoHub.Contracts.Vendors;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Application.Common.Pagination;

namespace SokoHub.Application.Modules.Vendors.Queries;

public record GetListQuery(PagedRequest PagedRequest) : IRequest<PagedResult<VendorResponse>>;

public sealed class GetListHandler : IRequestHandler<GetListQuery, PagedResult<VendorResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetListHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<VendorResponse>> Handle(GetListQuery request, CancellationToken cancellationToken)
    {
        // Simple specification to get all vendors
        var spec = new VendorListSpecification(request.PagedRequest);
        var vendors = await _unitOfWork.Repository<Vendor>().ListAsync(spec, cancellationToken);
        var total = await _unitOfWork.Repository<Vendor>().CountAsync(spec, cancellationToken);

        var response = vendors.Select(v => new VendorResponse(
            v.Id,
            v.UserId,
            v.BusinessName,
            v.TaxId.Value,
            v.CommissionRate.Value,
            v.Status.ToString())).ToList();

        return new PagedResult<VendorResponse>(response, total, request.PagedRequest);
    }
}

public class VendorListSpecification : Specification<Vendor>
{
    public VendorListSpecification(PagedRequest pagedRequest)
        : base(v => true)
    {
        // In a real system, we would apply pagination/sorting here.
        // Assuming Specification base class supports this.
    }
}
