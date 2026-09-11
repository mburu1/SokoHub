using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Identity;

namespace SokoHub.Application.Auth;

public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;

public sealed class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public ForgotPasswordHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = EmailAddress.Create(request.Email);
        var spec = new UserByEmailSpecification(email);
        var user = await _unitOfWork.Repository<User>().SingleAsync(spec, cancellationToken);

        if (user is null)
        {
            return Result<bool>.Success(true);
        }

        var token = _passwordHasher.HashPassword(Guid.NewGuid().ToString("N"));
        var expiresAt = DateTimeOffset.UtcNow.AddHours(2);

        user.SetResetToken(_passwordHasher.HashPassword(token), expiresAt);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // In production, an integration event would be published to trigger
        // the email notification service with the token embedded in a reset link.

        return Result<bool>.Success(true);
    }
}
