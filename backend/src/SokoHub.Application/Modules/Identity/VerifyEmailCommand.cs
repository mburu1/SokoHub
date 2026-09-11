using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Modules.Identity;
using SokoHub.Domain.Interfaces;

namespace SokoHub.Application.Modules.Identity;

public record VerifyEmailCommand(
    Guid UserId,
    string Token) : IRequest<Result>;

public sealed class VerifyEmailHandler : IRequestHandler<VerifyEmailCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public VerifyEmailHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Repository<User>().GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            return Result.Failure(new ApplicationError("user_not_found", "User not found."));
        }

        try
        {
            user.ConfirmEmail(request.Token);
        }
        catch (SokoHub.Domain.Common.Exceptions.DomainValidationException ex)
        {
            return Result.Failure(new ApplicationError(ex.Code, ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
