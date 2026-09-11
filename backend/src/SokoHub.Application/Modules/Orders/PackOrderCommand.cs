using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;

namespace SokoHub.Application.Modules.Orders;

public record PackOrderCommand(Guid VendorOrderId, string Note = "") : IRequest<Result<bool>>;

public sealed class PackOrderHandler : IRequestHandler<PackOrderCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public PackOrderHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(PackOrderCommand request, CancellationToken cancellationToken)
    {
        var vendorOrder = await _unitOfWork.Repository<VendorOrder>().GetByIdAsync(request.VendorOrderId, cancellationToken);
        if (vendorOrder is null)
        {
            return Result<bool>.Failure(new ApplicationError("vendor_order_not_found", $"Vendor order {request.VendorOrderId} was not found."));
        }

        try
        {
            vendorOrder.Pack();
        }
        catch (SokoHub.Domain.Common.Exceptions.DomainValidationException ex)
        {
            return Result<bool>.Failure(new ApplicationError(ex.Code, ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}