using MediatR;
using SokoHub.Contracts.Orders;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;

namespace SokoHub.Application.Modules.Orders.Queries;

public record GetOrderByIdQuery(Guid Id) : IRequest<OrderResponse>;

public sealed class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.Id, cancellationToken);

        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        return new OrderResponse(
            order.Id,
            order.Number.Value,
            order.CustomerId,
            order.Status.ToString(),
            order.GrandTotal.Value,
            order.Currency,
            DateTimeOffset.UtcNow); // Simplified
    }
}
