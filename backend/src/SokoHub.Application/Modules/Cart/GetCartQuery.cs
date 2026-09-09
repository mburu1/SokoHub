using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Modules.Cart;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Cart;

public record GetCartQuery() : IRequest<Result<Cart>>;

public sealed class GetCartHandler : IRequestHandler<GetCartQuery, Result<Cart>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetCartHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<Cart>> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<Cart>().GetByUserIdAsync(_currentUser.Id, cancellationToken);

        if (cart == null)
        {
            // Create a new cart if none exists
            cart = Cart.Create(_currentUser.Id);
            await _unitOfWork.Repository<Cart>().AddAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result<Cart>.Success(cart);
    }
}
