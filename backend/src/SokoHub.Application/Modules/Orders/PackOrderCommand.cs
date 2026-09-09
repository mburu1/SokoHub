using MediatR;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Orders;

public record PackOrderCommand(Guid OrderId) : IRequest<Result>;

public sealed class PackOrderHandler : IRequestHandler<PackOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public PackOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(PackOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result.Failure(new ApplicationError("order_not_found", "Order not found."));
        }

        try
        {
            order.Pack();
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("pack_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
