using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;
using SokoHub.Contracts.Payments;

namespace SokoHub.Application.Modules.Payments;

public record ProcessMpesaCallbackCommand(PaymentCallbackRequest Callback) : IRequest<Result>;

public sealed class ProcessMpesaCallbackHandler : IRequestHandler<ProcessMpesaCallbackCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;

    public ProcessMpesaCallbackHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(ProcessMpesaCallbackCommand request, CancellationToken cancellationToken)
    {
        var callback = request.Callback;
        var cacheKey = $"mpesa:checkout:{callback.CheckoutRequestId}";
        var paymentId = await _cacheService.GetAsync<Guid>(cacheKey, cancellationToken);

        if (paymentId is null)
        {
            return Result.Failure(new ApplicationError("payment_not_found", "Payment not found for the given CheckoutRequestId."));
        }

        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(paymentId, cancellationToken);
        if (payment == null)
        {
            return Result.Failure(new ApplicationError("payment_not_found", "Payment not found for the given CheckoutRequestId."));
        }

        if (callback.ResultCode == 0)
        {
            var paidAmount = callback.Amount.HasValue
                ? Money.Create(callback.Amount.Value, payment.Amount.Currency)
                : null;

            payment.MarkAsSucceeded(callback.MpesaReceiptNumber ?? callback.CheckoutRequestId, paidAmount);
        }
        else
        {
            payment.MarkAsFailed(callback.ResultDesc);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
