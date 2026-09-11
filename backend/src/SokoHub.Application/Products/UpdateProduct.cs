using MediatR;
using SokoHub.Contracts.Products;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;

namespace SokoHub.Application.Products;

public record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string Description,
    Guid CategoryId,
    Guid? BrandId = null) : IRequest<ProductResponse>;

public sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId, cancellationToken);
        if (product == null)
        {
            throw new KeyNotFoundException($"Product with ID {request.ProductId} was not found.");
        }

        product.UpdateDetails(request.Name, request.Description, request.BrandId);
        product.Recategorize(request.CategoryId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var variants = product.Variants.Select(v => new ProductVariantResponse(
            v.Id,
            v.Sku.Value,
            v.Price.Amount,
            v.WeightGrams,
            v.IsActive)).ToList();

        return new ProductResponse(
            product.Id,
            product.VendorId,
            product.CategoryId,
            product.BrandId,
            product.Name,
            product.Slug.Value,
            product.Description,
            product.Status.ToString(),
            variants);
    }
}
