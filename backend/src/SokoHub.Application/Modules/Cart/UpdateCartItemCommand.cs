using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Interfaces;
using DomainCart = SokoHub.Domain.Modules.Cart;

namespace SokoHub.Application.Modules.Cart;

public record UpdateCartItemCommand(
    Guid ProductVariantId,
    int Quantity) : IRequest<Result<DomainCart.Cart>>;

public sealed class UpdateCartItemHandler : IRequestHandler<UpdateCartItemCommand, Result<DomainCart.Cart>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateCartItemHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<DomainCart.Cart>> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<DomainCart.Cart>().SingleAsync(
            new CartByUserIdSpecification(_currentUser.Id), cancellationToken);

        if (cart == null)
        {
            return Result<DomainCart.Cart>.Failure(new ApplicationError("cart_not_found", "Cart not found."));
        }

        try
        {
            cart.ChangeQuantity(request.ProductVariantId, request.Quantity);
        }
        catch (SokoHub.Domain.Common.Exceptions.DomainValidationException ex)
        {
            return Result<DomainCart.Cart>.Failure(new ApplicationError(ex.Code, ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<DomainCart.Cart>.Success(cart);
    }
}