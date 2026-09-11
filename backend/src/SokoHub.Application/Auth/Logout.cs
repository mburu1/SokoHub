using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Identity;

namespace SokoHub.Application.Auth;

public record LogoutCommand(string RefreshToken) : IRequest<Result<bool>>;

public sealed class LogoutHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public LogoutHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var spec = new RefreshTokenByHashSpecificationForLogout(request.RefreshToken);
        var token = await _unitOfWork.Repository<RefreshToken>().SingleAsync(spec, cancellationToken);

        if (token is null)
        {
            return Result<bool>.Success(true);
        }

        token.Revoke("logout");
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}

public sealed class RefreshTokenByHashSpecificationForLogout : Specification<RefreshToken>
{
    public RefreshTokenByHashSpecificationForLogout(string tokenHash)
        : base(t => t.TokenHash == tokenHash)
    {
    }
}
