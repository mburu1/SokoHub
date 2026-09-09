using MediatR;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Orders;

public record DeliverOrderCommand(Guid OrderId) : IRequest<Result>;

public sealed class DeliverOrderHandler : IRequestHandler<DeliverOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeliverOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeliverOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure(new ApplicationError("order_not_found", "Order not found."));
        }

        try
        {
            order.Deliver();
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("deliver_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
