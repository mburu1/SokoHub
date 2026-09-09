using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Inventory;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Inventory;

public record ReleaseInventoryCommand(Guid ReservationId) : IRequest<Result>;

public sealed class ReleaseInventoryHandler : IRequestHandler<ReleaseInventoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReleaseInventoryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReleaseInventoryCommand request, CancellationToken cancellationToken)
    {
        var reservation = await _unitOfWork.Repository<InventoryReservation>().GetByIdAsync(request.ReservationId, cancellationToken);

        if (reservation == null)
        {
            return Result.Failure(new ApplicationError("reservation_not_found", "Inventory reservation not found."));
        }

        try
        {
            reservation.Release();
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("release_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
