using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Modules.Identity;
using SokoHub.Domain.Interfaces;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Identity;

public record ForgotPasswordCommand(string Email) : IRequest<Result>;

public sealed class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public ForgotPasswordHandler(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Repository<User>().GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            // Security: don't reveal if user exists
            return Result.Success();
        }

        var token = Guid.NewGuid().ToString();
        user.GeneratePasswordResetToken(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _notificationService.SendEmailAsync(user.Email, "Password Reset", $"Your token is {token}");

        return Result.Success();
    }
}
