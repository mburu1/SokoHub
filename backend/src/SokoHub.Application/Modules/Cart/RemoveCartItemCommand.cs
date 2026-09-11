using MediatR;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Interfaces;
using DomainCart = SokoHub.Domain.Modules.Cart;

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
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException("User not authenticated");
        var cart = await _unitOfWork.Repository<DomainCart.Cart>().SingleAsync(
            new CartByUserIdSpecification(userId), cancellationToken);

        if (cart == null)
        {
            return Result.Failure(new ApplicationError("cart_not_found", "Cart not found."));
        }

        try
        {
            cart.RemoveItem(request.ProductVariantId);
        }
        catch (SokoHub.Domain.Common.Exceptions.DomainValidationException ex)
        {
            return Result.Failure(new ApplicationError(ex.Code, ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}