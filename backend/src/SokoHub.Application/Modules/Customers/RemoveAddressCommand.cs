using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Customers;

namespace SokoHub.Application.Modules.Customers;

public record RemoveAddressCommand(Guid CustomerId, Guid AddressId) : IRequest<Result<CustomerResponse>>;

public sealed class RemoveAddressHandler : IRequestHandler<RemoveAddressCommand, Result<CustomerResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public RemoveAddressHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerResponse>> Handle(RemoveAddressCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return Result<CustomerResponse>.Failure(new ApplicationError("customer_not_found", $"Customer {request.CustomerId} was not found."));
        }

        customer.RemoveAddress(request.AddressId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CustomerResponse>.Success(Queries.GetByIdHandler.MapToResponse(customer));
    }
}
