using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Modules.Identity;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Identity;

public record ResetPasswordCommand(
    string Token,
    string NewPassword) : IRequest<Result>;

public sealed class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Repository<User>().GetByPasswordTokenAsync(request.Token, cancellationToken);

        if (user == null)
        {
            return Result.Failure(new ApplicationError("invalid_token", "The password reset token is invalid or has expired."));
        }

        var passwordHash = _passwordHasher.HashPassword(request.NewPassword);
        user.UpdatePassword(passwordHash);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
