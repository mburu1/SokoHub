using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Identity;

namespace SokoHub.Application.Auth;

public record VerifyEmailCommand(string Email, string Token) : IRequest<Result<bool>>;

public sealed class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public VerifyEmailHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<bool>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var email = EmailAddress.Create(request.Email);
        var spec = new UserByEmailSpecification(email);
        var user = await _unitOfWork.Repository<User>().SingleAsync(spec, cancellationToken);

        if (user is null)
        {
            return Result<bool>.Failure(new ApplicationError("auth_user_not_found", "User not found."));
        }

        if (user.EmailVerificationTokenHash is null)
        {
            user.Verify();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }

        var tokenHash = _passwordHasher.HashPassword(request.Token);
        user.ConfirmEmail(tokenHash);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
