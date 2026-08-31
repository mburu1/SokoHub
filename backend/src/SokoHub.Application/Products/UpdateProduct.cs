using MediatR;
using SokoHub.Contracts.Products;
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
        // var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId);
        // if (product == null) throw new NotFoundException("Product not found");

        // product.UpdateProfile(request.Name, request.Description); // Assuming a helper method
        // product.Recategorize(request.CategoryId);

        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        throw new NotImplementedException("UpdateProduct requires repository connectivity.");
    }
}
