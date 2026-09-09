using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Modules.Cart;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Cart;

public record ClearCartCommand() : IRequest<Result>;

public sealed class ClearCartHandler : IRequestHandler<ClearCartCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public ClearCartHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Repository<Cart>().GetByUserIdAsync(_currentUser.Id, cancellationToken);
        if (cart == null)
        {
            return Result.Success();
        }

        cart.Clear();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
