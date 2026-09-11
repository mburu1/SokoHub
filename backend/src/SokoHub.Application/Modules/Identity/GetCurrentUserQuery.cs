using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Modules.Identity;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Identity;

public record GetCurrentUserQuery() : IRequest<Result<User>>;

public sealed class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<User>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public GetCurrentUserHandler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<Result<User>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.Id ?? throw new UnauthorizedAccessException("User not authenticated");
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            return Result<User>.Failure(new ApplicationError("user_not_found", "Current user not found."));
        }

        return Result<User>.Success(user);
    }
}
