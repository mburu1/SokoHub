using MediatR;
using SokoHub.Contracts.Payments;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Payments;

public record CreatePaymentCommand(
    Guid OrderId,
    Guid CustomerId,
    Money Amount,
    PaymentMethod Method) : IRequest<PaymentResponse>;

public sealed class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, PaymentResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.Create(
            request.OrderId,
            request.CustomerId,
            request.Amount,
            request.Method);

        await _unitOfWork.Repository<Payment>().AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new PaymentResponse(
            payment.Id,
            payment.OrderId,
            payment.Amount.Value,
            payment.Amount.Currency,
            payment.Method.ToString(),
            payment.Status.ToString(),
            payment.Reference.Value);
    }
}
