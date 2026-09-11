using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Auth;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Identity;

namespace SokoHub.Application.Auth;

public record GetCurrentUserQuery(Guid UserId) : IRequest<Result<UserDto>>;

public sealed class GetCurrentUserHandler : IRequestHandler<GetCurrentUserQuery, Result<UserDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCurrentUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UserDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return Result<UserDto>.Failure(new ApplicationError("user_not_found", "User not found."));
        }

        var roles = await _unitOfWork.Repository<Role>().ListAsync(
            new UserRolesSpecification(user.RoleIds), cancellationToken);
        var roleNames = roles.Select(r => r.Name).ToList();

        var userDto = new UserDto(
            user.Id,
            user.Email.Value,
            user.Phone.E164,
            user.DisplayName,
            roleNames,
            user.Status == UserStatus.Active,
            user.CreatedAt);

        return Result<UserDto>.Success(userDto);
    }
}

public sealed class UserRolesSpecification : Specification<Role>
{
    public UserRolesSpecification(IReadOnlyList<Guid> roleIds)
        : base(r => roleIds.Contains(r.Id))
    {
    }
}
