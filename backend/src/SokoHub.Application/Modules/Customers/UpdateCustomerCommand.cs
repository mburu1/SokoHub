using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Common;
using SokoHub.Contracts.Customers;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Customers;

namespace SokoHub.Application.Modules.Customers;

public record UpdateCustomerCommand(
    Guid Id,
    string? Email = null,
    string? Phone = null,
    string? DisplayName = null) : IRequest<Result<CustomerResponse>>;

public sealed class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, Result<CustomerResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerResponse>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return Result<CustomerResponse>.Failure(new ApplicationError("customer_not_found", $"Customer {request.Id} was not found."));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CustomerResponse>.Success(Queries.GetByIdHandler.MapToResponse(customer));
    }
}
