using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Shipping;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Shipping;

public record CreateShipmentCommand(Guid OrderId) : IRequest<Result<Guid>>;

public sealed class CreateShipmentHandler : IRequestHandler<CreateShipmentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateShipmentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);

        if (order == null)
        {
            return Result<Guid>.Failure(new ApplicationError("order_not_found", "Order not found."));
        }

        var shipment = Shipment.Create(order);

        await _unitOfWork.Repository<Shipment>().AddAsync(shipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(shipment.Id);
    }
}
