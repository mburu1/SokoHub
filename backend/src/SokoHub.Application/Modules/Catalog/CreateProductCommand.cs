using MediatR;
using SokoHub.Contracts.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Catalog;

public record CreateProductCommand(
    Guid VendorId,
    Guid CategoryId,
    string Name,
    string Description,
    Guid? BrandId = null) : IRequest<ProductResponse>;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateProductHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Ensure user is the vendor or admin
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor == null)
        {
            throw new KeyNotFoundException("Vendor not found.");
        }

        if (vendor.UserId != _currentUser.Id)
        {
            throw new UnauthorizedAccessException("You can only create products for your own vendor account.");
        }

        var product = Product.Create(
            request.VendorId,
            request.CategoryId,
            request.Name,
            request.Description,
            request.BrandId);

        await _unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ProductResponse(
            product.Id,
            product.VendorId,
            product.CategoryId,
            product.BrandId,
            product.Name,
            product.Slug.Value,
            product.Description,
            product.Status.ToString());
    }
}
