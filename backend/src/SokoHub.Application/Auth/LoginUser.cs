using MediatR;
using SokoHub.Contracts.Auth;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Identity;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Application.Auth;

public record LoginUserCommand(
    string Email,
    string Password) : IRequest<Result<AuthResponse>>;

public sealed class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public LoginUserHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var email = EmailAddress.Create(request.Email);
        var spec = new UserByEmailSpecification(email);
        var user = await _unitOfWork.Repository<User>().SingleAsync(spec, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure(new ApplicationError("auth_invalid_credentials", "Invalid email or password"));
        }

        user.RecordSuccessfulAccess();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtProvider.GenerateToken(user);
        var refresh = user.IssueRefreshToken(_jwtProvider.GenerateRefreshToken(), DateTimeOffset.UtcNow.AddDays(7));

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refresh, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AuthResponse>.Success(new AuthResponse(
            token.AccessToken,
            refresh.TokenHash,
            new[] { token.Expiration },
            user.Id,
            user.Email.Value));
    }
}

public class UserByEmailSpecification : Specification<User>
{
    public UserByEmailSpecification(EmailAddress email)
        : base(u => u.Email == email)
    {
    }
}
