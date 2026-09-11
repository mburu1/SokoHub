using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Shipping;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Modules.Vendors;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Common.ValueObjects;

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

        var firstVendorOrder = order.VendorOrders.FirstOrDefault();
        if (firstVendorOrder == null)
        {
            return Result<Guid>.Failure(new ApplicationError("vendor_order_not_found", "No vendor order found."));
        }

        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(firstVendorOrder.VendorId, cancellationToken);
        if (vendor == null)
        {
            return Result<Guid>.Failure(new ApplicationError("vendor_not_found", "Vendor not found."));
        }

        var origin = Address.Create("SokoHub Warehouse", "Nairobi", "Nairobi", "00100", "KE");
        var destination = order.ShippingAddress;
        var cost = Money.Zero("KES");

        var shipment = Shipment.Create(
            order.Id,
            vendor.Id,
            Guid.Empty, // courier will be assigned later
            origin,
            destination,
            cost);

        await _unitOfWork.Repository<Shipment>().AddAsync(shipment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(shipment.Id);
    }
}
