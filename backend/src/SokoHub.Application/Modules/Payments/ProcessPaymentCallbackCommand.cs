using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Application.Modules.Payments;

public record ProcessPaymentCallbackCommand(
    Guid PaymentId,
    string CheckoutRequestId,
    int ResultCode,
    string ResultDescription,
    string? MpesaReceiptNumber,
    Money? PaidAmount) : IRequest<bool>;

public sealed class ProcessPaymentCallbackHandler : IRequestHandler<ProcessPaymentCallbackCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentCallbackHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(ProcessPaymentCallbackCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment == null)
        {
            throw new KeyNotFoundException("Payment not found.");
        }

        payment.ApplyCallback(
            request.CheckoutRequestId,
            request.ResultCode,
            request.ResultDescription,
            request.MpesaReceiptNumber,
            request.PaidAmount);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
