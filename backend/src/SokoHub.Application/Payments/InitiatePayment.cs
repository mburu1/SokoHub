using MediatR;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Payments;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Modules.Payments;

namespace SokoHub.Application.Payments;

public record InitiatePaymentCommand(
    Guid OrderId,
    string PhoneNumber,
    decimal Amount,
    string PaymentMethod = "Mpesa",
    string Currency = "KES") : IRequest<Result<PaymentResponse>>;

public sealed class InitiatePaymentHandler : IRequestHandler<InitiatePaymentCommand, Result<PaymentResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMpesaService _mpesaService;
    private readonly ICacheService _cacheService;

    public InitiatePaymentHandler(
        IUnitOfWork unitOfWork,
        IMpesaService mpesaService,
        ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _mpesaService = mpesaService;
        _cacheService = cacheService;
    }

    public async Task<Result<PaymentResponse>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Repository<Order>().GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
        {
            return Result<PaymentResponse>.Failure(new ApplicationError("order_not_found", "Order not found."));
        }

        var amount = Money.Create(request.Amount, request.Currency);
        var payment = Payment.Create(
            order.Id,
            order.CustomerId,
            amount,
            Domain.Modules.Payments.PaymentMethod.MpesaStk);

        // Call Daraja M-Pesa STK Push
        var stkResponse = await _mpesaService.InitiateStkPushAsync(
            request.PhoneNumber,
            request.Amount,
            order.Number.Value,
            $"Payment for {order.Number.Value}",
            cancellationToken);

        payment.InitiateMpesaStk(
            PhoneNumber.Create(request.PhoneNumber),
            stkResponse.CheckoutRequestId,
            stkResponse.MerchantRequestId);

        // Store checkout request ID to Redis with TTL for idempotency & reconciliation
        await _cacheService.SetAsync(
            $"mpesa:checkout:{stkResponse.CheckoutRequestId}",
            payment.Id,
            TimeSpan.FromMinutes(10),
            cancellationToken);

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
