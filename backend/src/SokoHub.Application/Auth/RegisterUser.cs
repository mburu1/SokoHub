using MediatR;
using SokoHub.Contracts.Auth;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Identity;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Application.Auth;

public record RegisterUserCommand(
    string Email,
    string Phone,
    string DisplayName,
    string Password) : IRequest<AuthResponse>;

public sealed class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

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

        await _unitOfWork.Repository<User>().AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _jwtProvider.GenerateToken(user);
        var refresh = user.IssueRefreshToken(_jwtProvider.GenerateRefreshToken(), DateTimeOffset.UtcNow.AddDays(7));

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refresh, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            token.AccessToken,
            refresh.TokenHash,
            new[] { token.Expiration },
            user.Id,
            user.Email.Value);
    }
}
