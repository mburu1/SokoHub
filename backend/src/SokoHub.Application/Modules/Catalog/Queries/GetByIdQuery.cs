using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Contracts.Catalog;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Application.Modules.Catalog.Queries;

public record GetByIdQuery(Guid Id) : IRequest<Result<ProductResponse>>;

public sealed class GetByIdHandler : IRequestHandler<GetByIdQuery, Result<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductResponse>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            return Result<ProductResponse>.Failure(new ApplicationError("product_not_found", $"Product with ID {request.Id} not found."));
        }

        return Result<ProductResponse>.Success(new ProductResponse(
            product.Id,
            product.Name,
            product.Slug.Value,
            product.Description,
            product.Status.ToString(),
            product.VendorId,
            product.CategoryId));
    }
}
