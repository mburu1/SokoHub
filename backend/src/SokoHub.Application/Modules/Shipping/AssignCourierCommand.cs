using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Shipping;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Shipping;

public record AssignCourierCommand(
    Guid ShipmentId,
    Guid CourierId) : IRequest<Result>;

public sealed class AssignCourierHandler : IRequestHandler<AssignCourierCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignCourierHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AssignCourierCommand request, CancellationToken cancellationToken)
    {
        var shipment = await _unitOfWork.Repository<Shipment>().GetByIdAsync(request.ShipmentId, cancellationToken);

        if (shipment == null)
        {
            return Result.Failure(new ApplicationError("shipment_not_found", "Shipment not found."));
        }

        var courier = await _unitOfWork.Repository<Courier>().GetByIdAsync(request.CourierId, cancellationToken);

        if (courier == null)
        {
            return Result.Failure(new ApplicationError("courier_not_found", "Courier not found."));
        }

        shipment.AssignCourier(courier);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
