using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;

namespace SokoHub.Application.Modules.Orders;

public record CancelOrderCommand(Guid OrderId, string Reason) : IRequest<Result<bool>>;

public sealed class CancelOrderHandler : IRequestHandler<CancelOrderCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<bool>.Failure(new ApplicationError("order_not_found", $"Order {request.OrderId} was not found."));
        }

        try
        {
            order.Cancel(request.Reason);
        }
        catch (SokoHub.Domain.Common.Exceptions.DomainValidationException ex)
        {
            return Result<bool>.Failure(new ApplicationError(ex.Code, ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
