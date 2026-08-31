using MediatR;
using SokoHub.Contracts.Auth;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Modules.Identity;

namespace SokoHub.Application.Auth;

public record RefreshTokenCommand(
    string RefreshToken) : IRequest<RefreshTokenResponse>;

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtProvider _jwtProvider;

    public RefreshTokenHandler(IUnitOfWork unitOfWork, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _jwtProvider = jwtProvider;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var spec = new RefreshTokenByHashSpecification(request.RefreshToken);
        var token = await _unitOfWork.Repository<RefreshToken>().SingleAsync(spec, cancellationToken);

        if (token == null || !token.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var user = await _unitOfWork.Repository<User>().GetByIdAsync(token.UserId, cancellationToken);
        if (user == null) throw new UnauthorizedAccessException("User not found.");

        var newToken = _jwtProvider.GenerateToken(user);
        var newRefresh = user.IssueRefreshToken(_jwtProvider.GenerateRefreshToken(), DateTimeOffset.UtcNow.AddDays(7));

        // TODO: Update old token to revoked and save new one
        // token.Revoke("refreshed");
        // await _unitOfWork.Repository<RefreshToken>().AddAsync(newRefresh, cancellationToken);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponse(newToken.AccessToken, newRefresh.TokenHash);
    }
}

public class RefreshTokenByHashSpecification : Specification<RefreshToken>
{
    public RefreshTokenByHashSpecification(string hash)
        : base(t => t.TokenHash == hash)
    {
    }
}
