using MediatR;
using SokoHub.Contracts.Auth;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Modules.Identity;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Application.Auth;

public record RefreshTokenCommand(
    string RefreshToken) : IRequest<Result<RefreshTokenResponse>>;

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtProvider _jwtProvider;

    public RefreshTokenHandler(IUnitOfWork unitOfWork, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var spec = new RefreshTokenByHashSpecification(request.RefreshToken);
        var token = await _unitOfWork.Repository<RefreshToken>().SingleAsync(spec, cancellationToken);

        if (token == null || !token.IsActive)
        {
            return Result<RefreshTokenResponse>.Failure(new ApplicationError("auth_invalid_token", "Invalid or expired refresh token."));
        }

        var user = await _unitOfWork.Repository<User>().GetByIdAsync(token.UserId, cancellationToken);
        if (user == null)
        {
            return Result<RefreshTokenResponse>.Failure(new ApplicationError("auth_user_not_found", "User associated with token not found."));
        }

        var newToken = _jwtProvider.GenerateToken(user);
        var newRefresh = user.IssueRefreshToken(_jwtProvider.GenerateRefreshToken(), DateTimeOffset.UtcNow.AddDays(7));

        token.Revoke("refreshed");
        await _unitOfWork.Repository<RefreshToken>().AddAsync(newRefresh, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse(newToken.AccessToken, newRefresh.TokenHash));
    }
}

public class RefreshTokenByHashSpecification : Specification<RefreshToken>
{
    public RefreshTokenByHashSpecification(string hash)
        : base(t => t.TokenHash == hash)
    {
    }
}
