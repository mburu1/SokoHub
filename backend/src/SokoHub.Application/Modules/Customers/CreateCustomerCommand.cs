using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Customers;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Customers;

namespace SokoHub.Application.Modules.Customers;

public record CreateCustomerCommand(
    Guid UserId,
    string Email,
    string Phone) : IRequest<Result<CustomerResponse>>;

public sealed class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerResponse>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var existing = await _unitOfWork.Repository<Customer>().SingleAsync(
            new CustomerByUserIdSpecification(request.UserId), cancellationToken);

        if (existing is not null)
        {
            return Result<CustomerResponse>.Failure(new ApplicationError("customer_exists", "Customer profile already exists."));
        }

        var customer = Customer.Create(
            request.UserId,
            EmailAddress.Create(request.Email),
            PhoneNumber.Create(request.Phone));

        await _unitOfWork.Repository<Customer>().AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CustomerResponse>.Success(Queries.GetByIdHandler.MapToResponse(customer));
    }
}

public sealed class CustomerByUserIdSpecification : Specification<Customer>
{
    public CustomerByUserIdSpecification(Guid userId)
        : base(c => c.UserId == userId)
    {
    }
}
