using MediatR;
using SokoHub.Contracts.Products;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Catalog;

namespace SokoHub.Application.Products;

public record CreateProductCommand(
    Guid VendorId,
    Guid CategoryId,
    string Name,
    string Description,
    Guid? BrandId = null) : IRequest<ProductResponse>;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = Product.Create(
            request.VendorId,
            request.CategoryId,
            request.Name,
            request.Description,
            request.BrandId);

        // await _unitOfWork.Repository<Product>().AddAsync(product);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductResponse(
            product.Id,
            product.VendorId,
            product.CategoryId,
            product.BrandId,
            product.Name,
            product.Slug.Value,
            product.Description,
            product.Status.ToString(),
            []); // Variants would be added in separate commands
    }
}
