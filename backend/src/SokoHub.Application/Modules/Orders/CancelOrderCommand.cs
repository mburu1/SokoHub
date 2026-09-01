using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Orders;

public record CancelOrderCommand(
    Guid OrderId,
    string Reason) : IRequest<bool>;

public sealed class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        order.Cancel(request.Reason);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
