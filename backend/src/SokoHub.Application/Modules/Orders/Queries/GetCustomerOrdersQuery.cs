using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Pagination;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Orders;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;

namespace SokoHub.Application.Modules.Orders.Queries;

public record GetCustomerOrdersQuery(Guid CustomerId, int Page = 1, int PageSize = 20)
    : IRequest<Result<PagedResult<OrderResponse>>>;

public sealed class GetCustomerOrdersHandler : IRequestHandler<GetCustomerOrdersQuery, Result<PagedResult<OrderResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerOrdersHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<OrderResponse>>> Handle(GetCustomerOrdersQuery request, CancellationToken cancellationToken)
    {
        var spec = new OrdersByCustomerSpecification(request.CustomerId)
            .ApplyOrderByDescending(o => o.CreatedAt)
            .ApplyPaging((request.Page - 1) * request.PageSize, request.PageSize);

        var orders = await _unitOfWork.Repository<Order>().ListAsync(spec, cancellationToken);
        var count = await _unitOfWork.Repository<Order>().CountAsync(
            new OrdersByCustomerSpecification(request.CustomerId), cancellationToken);

        var items = orders.Select(MapToResponse).ToList();
        var paged = new PagedResult<OrderResponse>(items, count, request.Page, request.PageSize);

        return Result<PagedResult<OrderResponse>>.Success(paged);
    }

    private static OrderResponse MapToResponse(Order order) =>
        new(
            order.Id,
            order.Number.Value,
            order.CustomerId,
            order.Status.ToString(),
            order.GrandTotal.Amount,
            order.Currency,
            order.CreatedAt);
}

public sealed class OrdersByCustomerSpecification : Specification<Order>
{
    public OrdersByCustomerSpecification(Guid customerId)
        : base(o => o.CustomerId == customerId)
    {
    }
}
