using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Payments;

public record RefundPaymentCommand(
    Guid PaymentId,
    Money RefundAmount,
    string Reason) : IRequest<Result>;

public sealed class RefundPaymentHandler : IRequestHandler<RefundPaymentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public RefundPaymentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment == null)
        {
            return Result.Failure(new ApplicationError("payment_not_found", "Payment not found."));
        }

        try
        {
            payment.Refund(request.RefundAmount, request.Reason);
        }
        catch (Exception ex)
        {
            return Result.Failure(new ApplicationError("refund_failed", ex.Message));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
