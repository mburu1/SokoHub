using MediatR;
using SokoHub.Contracts.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Application.Common.Pagination;
using SokoHub.Application.Common.Results;

namespace SokoHub.Application.Modules.Catalog;

public record SearchProductsQuery(
    string? Query,
    Guid? CategoryId,
    Guid? BrandId,
    PagedRequest PagedRequest) : IRequest<Result<PagedResult<ProductResponse>>>;

public sealed class SearchProductsHandler : IRequestHandler<SearchProductsQuery, Result<PagedResult<ProductResponse>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchProductsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ProductResponse>>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var spec = new ProductSearchSpecification(request.Query, request.CategoryId, request.BrandId, request.PagedRequest);
        var products = await _unitOfWork.Repository<Product>().ListAsync(spec, cancellationToken);
        var total = await _unitOfWork.Repository<Product>().CountAsync(spec, cancellationToken);

        var response = products.Select(p => new ProductResponse(
             p.Id,
             p.VendorId,
             p.CategoryId,
             p.BrandId,
             p.Name,
             p.Slug.Value,
             p.Description,
             p.Status.ToString())).ToList();

        return Result<PagedResult<ProductResponse>>.Success(new PagedResult<ProductResponse>(response, total, request.PagedRequest.Page, request.PagedRequest.PageSize));
    }
}

    public class ProductSearchSpecification : Specification<Product>
    {
        public ProductSearchSpecification(string? query, Guid? categoryId, Guid? brandId, PagedRequest pagedRequest)
            : base(p =>
                (string.IsNullOrEmpty(query) || p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)) &&
                (!categoryId.HasValue || p.CategoryId == categoryId) &&
                (!brandId.HasValue || p.BrandId == brandId))
        {
            ApplyOrderByDescending(p => p.CreatedAt);
            ApplyPaging(pagedRequest.Page, pagedRequest.PageSize);
        }
    }
