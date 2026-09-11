using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Customers;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Customers;

namespace SokoHub.Application.Modules.Customers;

public record AddAddressCommand(
    Guid CustomerId,
    string Line1,
    string City,
    string County,
    string? Line2 = null,
    string? PostalCode = null,
    string? Country = "KE",
    bool IsDefault = false) : IRequest<Result<CustomerResponse>>;

public sealed class AddAddressHandler : IRequestHandler<AddAddressCommand, Result<CustomerResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddAddressHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerResponse>> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result<CustomerResponse>.Failure(new ApplicationError("customer_not_found", $"Customer {request.CustomerId} was not found."));
        }

        var address = new Address(request.Line1, request.City, request.County, request.Line2, request.PostalCode, request.Country);
        var customerAddress = new CustomerAddress(Guid.NewGuid(), request.CustomerId, address, request.IsDefault);

        customer.AddAddress(customerAddress);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CustomerResponse>.Success(Queries.GetByIdHandler.MapToResponse(customer));
    }
}
