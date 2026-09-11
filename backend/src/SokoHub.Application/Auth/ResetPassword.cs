using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Identity;

namespace SokoHub.Application.Auth;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result<bool>>;

public sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = EmailAddress.Create(request.Email);
        var spec = new UserByEmailSpecification(email);
        var user = await _unitOfWork.Repository<User>().SingleAsync(spec, cancellationToken);

        if (user is null)
        {
            return Result<bool>.Failure(new ApplicationError("auth_user_not_found", "User not found."));
        }

        try
        {
            var tokenHash = _passwordHasher.HashPassword(request.Token);
            user.ConsumeResetToken(tokenHash);
            user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is SokoHub.Domain.Common.Exceptions.DomainException || ex is SokoHub.Domain.Common.Exceptions.InvariantViolationException)
        {
            return Result<bool>.Failure(new ApplicationError("reset_token_invalid", "Reset token is invalid or expired."));
        }

        return Result<bool>.Success(true);
    }
}
