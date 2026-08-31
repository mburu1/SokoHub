using MediatR;
using SokoHub.Contracts.Products;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Catalog;

namespace SokoHub.Application.Products;

public record AddProductVariantCommand(
    Guid ProductId,
    string Sku,
    decimal Price,
    int? WeightGrams = null) : IRequest<ProductVariantResponse>;

public sealed class AddProductVariantHandler : IRequestHandler<AddProductVariantCommand, ProductVariantResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public AddProductVariantHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductVariantResponse> Handle(AddProductVariantCommand request, CancellationToken cancellationToken)
    {
        // var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId);
        // if (product == null) throw new NotFoundException("Product not found");

        // var sku = Sku.Create(request.Sku);
        // var price = ProductPrice.Create(request.Price);
        // var variant = product.AddVariant(sku, price, request.WeightGrams);

        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        // return new ProductVariantResponse(variant.Id, variant.Sku.Value, variant.Price.Value, variant.WeightGrams, variant.IsActive);

        throw new NotImplementedException("AddProductVariant requires repository connectivity.");
    }
}
