using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Pagination;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Common;
using SokoHub.Contracts.Customers;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Customers;

namespace SokoHub.Application.Modules.Customers.Queries;

public record GetByIdQuery(Guid Id) : IRequest<Result<CustomerResponse>>;

public sealed class GetByIdHandler : IRequestHandler<GetByIdQuery, Result<CustomerResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerResponse>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _unitOfWork.Repository<Customer>().GetByIdAsync(request.Id, cancellationToken);
        if (customer is null)
        {
            return Result<CustomerResponse>.Failure(new ApplicationError("customer_not_found", $"Customer {request.Id} was not found."));
        }

        return Result<CustomerResponse>.Success(MapToResponse(customer));
    }

    internal static CustomerResponse MapToResponse(Customer customer) =>
        new(
            customer.Id,
            customer.UserId,
            customer.Email.Value,
            customer.Phone.Value,
            customer.Addresses.Select(a => new AddressDto(
                a.Address.Line1,
                a.Address.City,
                a.Address.County,
                a.Address.PostalCode,
                a.Address.CountryCode)).ToList());
}

public record GetListQuery(int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<CustomerResponse>>>;

public sealed class GetListHandler : IRequestHandler<GetListQuery, Result<PagedResult<CustomerResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetListHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<CustomerResponse>>> Handle(GetListQuery request, CancellationToken cancellationToken)
    {
        var spec = new Specification<Customer>()
            .ApplyOrderByDescending(c => c.CreatedAt)
            .ApplyPaging((request.Page - 1) * request.PageSize, request.PageSize);

        var customers = await _unitOfWork.Repository<Customer>().ListAsync(spec, cancellationToken);
        var count = await _unitOfWork.Repository<Customer>().CountAsync(
            new Specification<Customer>(), cancellationToken);

        var items = customers.Select(GetByIdHandler.MapToResponse).ToList();
        var paged = new PagedResult<CustomerResponse>(items, count, request.Page, request.PageSize);

        return Result<PagedResult<CustomerResponse>>.Success(paged);
    }
}