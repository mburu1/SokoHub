using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Modules.Vendors;

namespace SokoHub.Application.Modules.Catalog;

public record PublishProductCommand(Guid ProductId) : IRequest<Result>;

public sealed class PublishProductHandler : IRequestHandler<PublishProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public PublishProductHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(PublishProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            return Result.Failure(new ApplicationError("product_not_found", "Product not found."));
        }

        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(product.VendorId, cancellationToken);
        if (vendor == null || vendor.UserId != _currentUser.Id)
        {
            return Result.Failure(new ApplicationError("unauthorized", "You can only publish your own products."));
        }

        try
        {
            product.Publish();
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("publish_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
