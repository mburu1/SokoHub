using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;
using SokoHub.Contracts.Payments;

namespace SokoHub.Application.Modules.Payments;

public record ProcessMpesaCallbackCommand(PaymentCallbackRequest Callback) : IRequest<Result>;

public sealed class ProcessMpesaCallbackHandler : IRequestHandler<ProcessMpesaCallbackCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProcessMpesaCallbackHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ProcessMpesaCallbackCommand request, CancellationToken cancellationToken)
    {
        var callback = request.Callback;
        var payment = await _unitOfWork.Repository<Payment>().GetByReferenceAsync(callback.CheckoutRequestId, cancellationToken);

        if (payment == null)
        {
            return Result.Failure(new ApplicationError("payment_not_found", "Payment not found for the given CheckoutRequestId."));
        }

        if (callback.ResultCode == 0)
        {
            payment.MarkAsSucceeded(callback.MpesaReceiptNumber, callback.Value);
        }
        else
        {
            payment.MarkAsFailed(callback.ResultDesc);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
