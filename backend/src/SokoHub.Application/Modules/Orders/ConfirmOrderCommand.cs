using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Orders;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<bool>;

public sealed class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            throw new KeyNotFoundException("Order not found.");
        }

        order.Confirm();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
