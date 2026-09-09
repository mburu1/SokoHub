using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Orders;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<Result>;

public sealed class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure(new ApplicationError("order_not_found", "Order not found."));
        }

        try
        {
            order.Confirm();
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("confirm_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
