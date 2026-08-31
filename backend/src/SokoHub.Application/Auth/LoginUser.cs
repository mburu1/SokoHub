using MediatR;
using SokoHub.Contracts.Auth;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Identity;

namespace SokoHub.Application.Auth;

public record LoginUserCommand(
    string Email,
    string Password) : IRequest<AuthResponse>;

public sealed class LoginUserHandler : IRequestHandler<LoginUserCommand, AuthResponse>
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

    public async Task<AuthResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var email = EmailAddress.Create(request.Email);
        // var user = await _unitOfWork.Repository<User>().GetByEmailAsync(email);
        // if (user == null || !_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        // {
        //     throw new UnauthorizedAccessException("Invalid credentials");
        // }

        // user.RecordSuccessfulAccess();
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        // var token = _jwtProvider.GenerateToken(user);
        // var refresh = user.IssueRefreshToken(_jwtProvider.GenerateRefreshToken(), DateTimeOffset.UtcNow.AddDays(7));

        // return new AuthResponse(
        //     token.AccessToken,
        //     refresh.TokenHash,
        //     new[] { token.Expiration },
        //     user.Id,
        //     user.Email.Value);

        throw new NotImplementedException("Login implementation requires repository connectivity.");
    }
}
