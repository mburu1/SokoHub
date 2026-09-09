using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Modules.Cart;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Cart;

public record UpdateCartItemCommand(
    Guid ProductVariantId,
    int Quantity) : IRequest<Result<Cart>>;

public sealed class UpdateCartItemHandler : IRequestHandler<UpdateCartItemCommand, Result<Cart>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateCartItemHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<Cart>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<Cart>().GetByUserIdAsync(_currentUser.Id, cancellationToken);
        if (cart == null)
        {
            return Result<Cart>.Failure(new ApplicationError("cart_not_found", "Cart not found."));
        }

        cart.UpdateQuantity(request.ProductVariantId, request.Quantity);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Cart>.Success(cart);
    }
}
