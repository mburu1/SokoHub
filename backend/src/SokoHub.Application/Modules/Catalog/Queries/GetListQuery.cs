using MediatR;
using SokoHub.Application.Common.Pagination;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Catalog;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Application.Modules.Catalog.Queries;

public record GetListQuery(int Page = 1, int PageSize = 20, string? Search = null) : IRequest<Result<PagedResult<ProductResponse>>>;

public sealed class GetListHandler : IRequestHandler<GetListQuery, Result<PagedResult<ProductResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetListHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(GetListQuery request, CancellationToken cancellationToken)
    {
        var products = await _unitOfWork.Repository<Product>().ListAsync(cancellationToken);

        var filtered = products
            .Where(p => string.IsNullOrEmpty(request.Search) || p.Name.Contains(request.Search, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var totalCount = filtered.Count;
        var items = filtered
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductResponse(
                 p.Id,
                 p.VendorId,
                 p.CategoryId,
                 p.BrandId,
                 p.Name,
                 p.Slug.Value,
                 p.Description,
                 p.Status.ToString()))
            .ToList();

        return Result<PagedResult<ProductResponse>>.Success(new PagedResult<ProductResponse>(
            items,
            totalCount,
            request.Page,
            request.PageSize));
    }
}
