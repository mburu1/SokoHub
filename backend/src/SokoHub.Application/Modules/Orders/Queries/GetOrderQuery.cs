using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Orders;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;

namespace SokoHub.Application.Modules.Orders.Queries;

public record GetOrderQuery(Guid OrderId) : IRequest<Result<OrderResponse>>;

public sealed class GetOrderHandler : IRequestHandler<GetOrderQuery, Result<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<OrderResponse>.Failure(new ApplicationError("order_not_found", $"Order {request.OrderId} was not found."));
        }

        return Result<OrderResponse>.Success(MapToResponse(order));
    }

    private static OrderResponse MapToResponse(Order order) =>
        new(
            order.Id,
            order.Number.Value,
            order.CustomerId,
            order.Status.ToString(),
            order.GrandTotal.Amount,
            order.Currency,
            order.CreatedAt);
}
