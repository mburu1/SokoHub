using MediatR;
using SokoHub.Contracts.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Application.Common.Pagination;

namespace SokoHub.Application.Modules.Catalog;

public record SearchProductsQuery(
    string? Query,
    Guid? CategoryId,
    Guid? BrandId,
    PagedRequest PagedRequest) : IRequest<PagedResult<ProductResponse>>;

public sealed class SearchProductsHandler : IRequestHandler<SearchProductsQuery, PagedResult<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchProductsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ProductResponse>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
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

        return new PagedResult<ProductResponse>(response, total, request.PagedRequest);
    }
}

public class ProductSearchSpecification : Specification<Product>
{
    public ProductSearchSpecification(string? query, Guid? categoryId, Guid? brandId, PagedRequest pagedRequest)
        : base(p =>
            (string.IsNullOrEmpty(query) || p.Name.Contains(query)) &&
            (!categoryId.HasValue || p.CategoryId == categoryId) &&
            (!brandId.HasValue || p.BrandId == brandId))
    {
        // Pagination would be applied here in a real implementation.
    }
}
