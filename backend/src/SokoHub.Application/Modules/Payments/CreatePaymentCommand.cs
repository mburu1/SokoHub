using MediatR;
using SokoHub.Contracts.Payments;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;

namespace SokoHub.Application.Modules.Payments;

public record CreatePaymentCommand(
    Guid OrderId,
    Guid CustomerId,
    Money Amount,
    PaymentMethod Method) : IRequest<Result<PaymentResponse>>;

public sealed class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, Result<PaymentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaymentResponse>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.Create(
            request.OrderId,
            request.CustomerId,
            request.Amount,
            request.Method);

        await _unitOfWork.Repository<Payment>().AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PaymentResponse>.Success(new PaymentResponse(
            payment.Id,
            payment.OrderId,
            payment.Amount.Amount,
            payment.Amount.Currency,
            payment.Method.ToString(),
            payment.Status.ToString(),
            payment.Reference.Value));
    }
}
