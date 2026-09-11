using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Interfaces;
using DomainCart = SokoHub.Domain.Modules.Cart;
using SokoHub.Domain.Modules.Catalog;

namespace SokoHub.Application.Modules.Cart;

public record AddCartItemCommand(
    Guid ProductVariantId,
    int Quantity) : IRequest<Result<DomainCart.Cart>>;

public sealed class AddCartItemHandler : IRequestHandler<AddCartItemCommand, Result<DomainCart.Cart>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public AddCartItemHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<DomainCart.Cart>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<DomainCart.Cart>().SingleAsync(
            new CartByUserIdSpecification(_currentUser.Id), cancellationToken);

        if (cart == null)
        {
            cart = DomainCart.Cart.Create(_currentUser.Id);
            await _unitOfWork.Repository<DomainCart.Cart>().AddAsync(cart, cancellationToken);
        }

        var variant = await _unitOfWork.Repository<ProductVariant>().GetByIdAsync(request.ProductVariantId, cancellationToken);
        if (variant == null)
        {
            return Result<DomainCart.Cart>.Failure(new ApplicationError("variant_not_found", "Product variant not found."));
        }

        var product = await _unitOfWork.Repository<Product>().GetByIdAsync(variant.ProductId, cancellationToken);
        if (product == null)
        {
            return Result<DomainCart.Cart>.Failure(new ApplicationError("product_not_found", "Product not found."));
        }

        var unitPrice = variant.Price.EffectivePrice(DateTimeOffset.UtcNow);
        cart.AddItem(
            product.VendorId,
            product.Id,
            variant.Id,
            variant.Sku,
            product.Name,
            unitPrice,
            request.Quantity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DomainCart.Cart>.Success(cart);
    }
}
