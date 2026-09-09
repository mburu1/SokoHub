using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Common.Specifications;

namespace SokoHub.Application.Modules.Orders;

public record GetVendorOrdersQuery(Guid VendorId) : IRequest<Result<List<VendorOrder>>>;

public sealed class GetVendorOrdersHandler : IRequestHandler<GetVendorOrdersQuery, Result<List<VendorOrder>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetVendorOrdersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<VendorOrder>>> Handle(GetVendorOrdersQuery request, CancellationToken cancellationToken)
    {
        var spec = new VendorOrdersSpecification(request.VendorId);
        var orders = await _unitOfWork.Repository<VendorOrder>().ListAsync(spec, cancellationToken);

        return Result<List<VendorOrder>>.Success(orders);
    }
}

public class VendorOrdersSpecification : Specification<VendorOrder>
{
    public VendorOrdersSpecification(Guid vendorId)
        : base(vo => vo.VendorId == vendorId)
    {
    }
}
