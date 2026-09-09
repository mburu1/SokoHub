using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Payments;

public record ReversePaymentCommand(Guid PaymentId, string Reason) : IRequest<Result>;

public sealed class ReversePaymentHandler : IRequestHandler<ReversePaymentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReversePaymentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReversePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment == null)
        {
            return Result.Failure(new ApplicationError("payment_not_found", "Payment not found."));
        }

        try
        {
            payment.Reverse(request.Reason);
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("reverse_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
