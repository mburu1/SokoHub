using MediatR;
using SokoHub.Contracts.Auth;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Identity;

namespace SokoHub.Application.Auth;

public record RegisterUserCommand(
    string Email,
    string Phone,
    string DisplayName,
    string Password) : IRequest<AuthResponse>;

public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher; // Interface to be defined
    private readonly IJwtProvider _jwtProvider;       // Interface to be defined

    public RegisterUserHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task<AuthResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = EmailAddress.Create(request.Email);
        var phone = PhoneNumber.Create(request.Phone);
        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = User.Register(email, phone, request.DisplayName, passwordHash);

        // TODO: Add to repository and save
        // var userRepo = _unitOfWork.Repository<User>();
        // await userRepo.AddAsync(user);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtProvider.GenerateToken(user);
        var refresh = user.IssueRefreshToken(_jwtProvider.GenerateRefreshToken(), DateTimeOffset.UtcNow.AddDays(7));

        return new AuthResponse(
            token.AccessToken,
            refresh.TokenHash,
            new[] { token.Expiration },
            user.Id,
            user.Email.Value);
    }
}

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}

public interface IJwtProvider
{
    (string AccessToken, DateTime Expiration) GenerateToken(User user);
    string GenerateRefreshToken();
}
