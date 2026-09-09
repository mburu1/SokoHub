using MediatR;
using SokoHub.Contracts.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Catalog;

public record CreateProductCommand(
    Guid VendorId,
    Guid CategoryId,
    string Name,
    string Description,
    Guid? BrandId = null) : IRequest<Result<ProductResponse>>;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public CreateProductHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<ProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(request.VendorId, cancellationToken);
        if (vendor == null)
        {
            return Result<ProductResponse>.Failure(new ApplicationError("vendor_not_found", "Vendor not found."));
        }

        if (vendor.UserId != _currentUser.Id)
        {
            return Result<ProductResponse>.Failure(new ApplicationError("unauthorized", "You can only create products for your own vendor account."));
        }

        var product = Product.Create(
            request.VendorId,
            request.CategoryId,
            request.Name,
            request.Description,
            request.BrandId);

        await _unitOfWork.Repository<Product>().AddAsync(product, cancellationToken);
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
