using MediatR;
using SokoHub.Contracts.Auth;
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
        // var token = await _unitOfWork.Repository<RefreshToken>().GetByHashAsync(request.RefreshToken);
        // if (token == null || !token.IsActive) throw new UnauthorizedAccessException("Invalid token");

        // var user = await _unitOfWork.Repository<User>().GetByIdAsync(token.UserId);
        // var newToken = _jwtProvider.GenerateToken(user);
        // var newRefresh = user.IssueRefreshToken(_jwtProvider.GenerateRefreshToken(), DateTimeOffset.UtcNow.AddDays(7));

        // return new RefreshTokenResponse(newToken.AccessToken, newRefresh.TokenHash);

        throw new NotImplementedException("Refresh token implementation requires repository connectivity.");
    }
}
