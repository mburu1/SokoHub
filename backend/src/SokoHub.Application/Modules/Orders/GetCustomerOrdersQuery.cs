using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Common.Specifications;

namespace SokoHub.Application.Modules.Orders;

public record GetCustomerOrdersQuery(Guid CustomerId) : IRequest<Result<List<Order>>>;

public sealed class GetCustomerOrdersHandler : IRequestHandler<GetCustomerOrdersQuery, Result<List<Order>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerOrdersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<Order>>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        var spec = new OrdersByCustomerSpecification(request.CustomerId);
        var orders = await _unitOfWork.Repository<Order>().ListAsync(spec, cancellationToken);

        return Result<List<Order>>.Success(orders);
    }
}

public class OrdersByCustomerSpecification : Specification<Order>
{
    public OrdersByCustomerSpecification(Guid customerId)
        : base(o => o.CustomerId == customerId)
    {
    }
}
