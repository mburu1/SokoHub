using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Application.Modules.Orders;

public record GetOrderQuery(Guid Id) : IRequest<Result<Order>>;

public sealed class GetOrderHandler : IRequestHandler<GetOrderQuery, Result<Order>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Order>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.Id, cancellationToken);

        if (order == null)
        {
            return Result<Order>.Failure(new ApplicationError("order_not_found", "Order not found."));
        }

        return Result<Order>.Success(order);
    }
}
