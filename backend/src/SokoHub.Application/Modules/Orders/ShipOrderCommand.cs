using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;

namespace SokoHub.Application.Modules.Orders;

public record ShipOrderCommand(
    Guid OrderId,
    Guid VendorOrderId,
    string TrackingNumber) : IRequest<Result<bool>>;

public sealed class ShipOrderHandler : IRequestHandler<ShipOrderCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public ShipOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return Result<bool>.Failure(new ApplicationError("order_not_found", $"Order {request.OrderId} was not found."));
        }

        try
        {
            order.MarkVendorShipped(request.VendorOrderId, TrackingNumber.Parse(request.TrackingNumber));
        }
        catch (SokoHub.Domain.Common.Exceptions.DomainValidationException ex)
        {
            return Result<bool>.Failure(new ApplicationError(ex.Code, ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
