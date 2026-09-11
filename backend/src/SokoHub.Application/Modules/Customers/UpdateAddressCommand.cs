using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Customers;

namespace SokoHub.Application.Modules.Customers;

public record UpdateAddressCommand(
    Guid CustomerId,
    Guid AddressId,
    string? Line1 = null,
    string? City = null,
    string? County = null,
    string? Line2 = null,
    string? PostalCode = null,
    string? Country = null,
    bool? IsDefault = null) : IRequest<Result<CustomerResponse>>;

public sealed class UpdateAddressHandler : IRequestHandler<UpdateAddressCommand, Result<CustomerResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAddressHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerResponse>> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result<CustomerResponse>.Failure(new ApplicationError("customer_not_found", $"Customer {request.CustomerId} was not found."));
        }

        var address = customer.Addresses.FirstOrDefault(a => a.Id == request.AddressId);
        if (address is null)
        {
            return Result<CustomerResponse>.Failure(new ApplicationError("address_not_found", $"Address {request.AddressId} was not found."));
        }

        if (request.IsDefault == true)
        {
            address.SetAsDefault();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CustomerResponse>.Success(Queries.GetByIdHandler.MapToResponse(customer));
    }
}
