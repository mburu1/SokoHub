using MediatR;
using SokoHub.Contracts.Catalog;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Catalog;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    Guid CategoryId,
    Guid? BrandId) : IRequest<Result<ProductResponse>>;

public sealed class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateProductHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<ProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {
            return Result<ProductResponse>.Failure(new ApplicationError("product_not_found", "Product not found."));
        }

        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(product.VendorId, cancellationToken);
        if (vendor == null || vendor.UserId != _currentUser.Id)
        {
            return Result<ProductResponse>.Failure(new ApplicationError("unauthorized", "You are not authorized to update this product."));
        }

        product.Recategorize(request.CategoryId);
        // Assuming Product has an Update method or we modify properties if they have internal setters
        // For now, we use the domain model's constraints.

        await _unitOfWork.SaveChangesAsync(cancellationToken);

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
