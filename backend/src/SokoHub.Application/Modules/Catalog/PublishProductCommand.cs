using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Catalog;

public record PublishProductCommand(Guid ProductId) : IRequest<bool>;

public sealed class PublishProductHandler : IRequestHandler<PublishProductCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public PublishProductHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(PublishProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(request.ProductId, cancellationToken);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        // Ensure user owns the product
        var vendor = await _unitOfWork.Repository<Vendor>().GetByIdAsync(product.VendorId, cancellationToken);
        if (vendor == null || vendor.UserId != _currentUser.Id)
        {
            throw new UnauthorizedAccessException("You can only publish your own products.");
        }

        product.Publish();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
