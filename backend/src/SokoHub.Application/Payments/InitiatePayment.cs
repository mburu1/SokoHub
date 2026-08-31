using MediatR;
using SokoHub.Contracts.Payments;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Modules.Payments;

namespace SokoHub.Application.Payments;

public record InitiatePaymentCommand(
    Guid OrderId,
    string PhoneNumber,
    decimal Amount) : IRequest<PaymentResponse>;

public sealed class InitiatePaymentHandler : IRequestHandler<InitiatePaymentCommand, PaymentResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public InitiatePaymentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentResponse> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
    {
        // var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId);
        // if (order == null) throw new NotFoundException("Order not found");

        var payment = Payment.Create(
            request.OrderId,
            Guid.Empty, // CustomerId from Order
            Money.Create(request.Amount),
            PaymentMethod.MpesaStk);

        payment.InitiateMpesaStk(
            PhoneNumber.Create(request.PhoneNumber),
            Guid.NewGuid().ToString(), // CheckoutRequestId
            Guid.NewGuid().ToString()); // MerchantRequestId

        // await _unitOfWork.Repository<Payment>().AddAsync(payment);
        // await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PaymentResponse(
            payment.Id,
            payment.Reference.Value,
            payment.Status.ToString(),
            payment.Amount.Amount);
    }
}
