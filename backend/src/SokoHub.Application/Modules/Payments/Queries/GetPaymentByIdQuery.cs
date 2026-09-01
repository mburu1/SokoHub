using MediatR;
using SokoHub.Contracts.Payments;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;

namespace SokoHub.Application.Modules.Payments.Queries;

public record GetPaymentByIdQuery(Guid Id) : IRequest<PaymentResponse>;

public sealed class GetPaymentByIdHandler : IRequestHandler<GetPaymentByIdQuery, PaymentResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaymentResponse> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(request.Id, cancellationToken);

        if (payment == null)
        {
            throw new KeyNotFoundException("Payment not found.");
        }

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
