using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Modules.Cart;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Cart;

public record RemoveCartItemCommand(Guid ProductVariantId) : IRequest<Result>;

public sealed class RemoveCartItemHandler : IRequestHandler<RemoveCartItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public RemoveCartItemHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<Cart>().GetByUserIdAsync(_currentUser.Id, cancellationToken);
        if (cart == null)
        {
            return Result.Failure(new ApplicationError("cart_not_found", "Cart not found."));
        }

        cart.RemoveItem(request.ProductVariantId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
