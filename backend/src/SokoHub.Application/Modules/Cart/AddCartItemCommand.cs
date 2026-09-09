using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Modules.Cart;
using SokoHub.Domain.Modules.Catalog;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Application.Modules.Cart;

public record AddCartItemCommand(
    Guid ProductVariantId,
    int Quantity) : IRequest<Result<Cart>>;

public sealed class AddCartItemHandler : IRequestHandler<AddCartItemCommand, Result<Cart>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public AddCartItemHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<Cart>> Handle(AddCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<Cart>().GetByUserIdAsync(_currentUser.Id, cancellationToken);
        if (cart == null)
        {
            cart = Cart.Create(_currentUser.Id);
            await _unitOfWork.Repository<Cart>().AddAsync(cart, cancellationToken);
        }

        var variant = await _unitOfWork.Repository<ProductVariant>().GetByIdAsync(request.ProductVariantId, cancellationToken);
        if (variant == null)
        {
            return Result<Cart>.Failure(new ApplicationError("variant_not_found", "Product variant not found."));
        }

        cart.AddItem(variant.Id, request.Quantity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Cart>.Success(cart);
    }
}
