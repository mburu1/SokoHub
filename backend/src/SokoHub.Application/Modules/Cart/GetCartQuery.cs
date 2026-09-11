using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Interfaces;
using DomainCart = SokoHub.Domain.Modules.Cart;

namespace SokoHub.Application.Modules.Cart;

public record GetCartQuery() : IRequest<Result<DomainCart.Cart>>;

public sealed class GetCartHandler : IRequestHandler<GetCartQuery, Result<DomainCart.Cart>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetCartHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<DomainCart.Cart>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<DomainCart.Cart>().SingleAsync(
            new CartByUserIdSpecification(_currentUser.Id), cancellationToken);

        if (cart == null)
        {
            cart = DomainCart.Cart.Create(_currentUser.Id);
            await _unitOfWork.Repository<DomainCart.Cart>().AddAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<DomainCart.Cart>.Success(cart);
    }
}

public sealed class CartByUserIdSpecification : Specification<DomainCart.Cart>
{
    public CartByUserIdSpecification(Guid? userId)
        : base(c => c.CustomerId == userId)
    {
    }
}
