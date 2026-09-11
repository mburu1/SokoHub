using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;

namespace SokoHub.Application.Modules.Orders;

public record ConfirmOrderCommand(Guid OrderId) : IRequest<Result<bool>>;

public sealed class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<bool>.Failure(new ApplicationError("order_not_found", $"Order {request.OrderId} was not found."));
        }

        try
        {
            order.Confirm();
        }
        catch (SokoHub.Domain.Common.Exceptions.DomainValidationException ex)
        {
            return Result<bool>.Failure(new ApplicationError(ex.Code, ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
