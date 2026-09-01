using MediatR;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;
using SokoHub.Domain.Common.ValueObjects;

namespace SokoHub.Application.Modules.Payments;

public record RefundPaymentCommand(
    Guid PaymentId,
    Money RefundAmount,
    string Reason) : IRequest<bool>;

public sealed class RefundPaymentHandler : IRequestHandler<RefundPaymentCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public RefundPaymentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(request.PaymentId, cancellationToken);

        if (payment == null)
        {
            throw new KeyNotFoundException("Payment not found.");
        }

        payment.Refund(request.RefundAmount, request.Reason);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
