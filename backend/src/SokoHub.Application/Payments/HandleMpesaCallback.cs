using MediatR;
using Microsoft.Extensions.Logging;
using SokoHub.Application.Common.Errors;
using SokoHub.Application.Common.Interfaces;
using SokoHub.Application.Common.Results;
using SokoHub.Contracts.Events.Orders;
using SokoHub.Contracts.Events.Payments;
using SokoHub.Contracts.Payments;
using SokoHub.Domain.Common.Specifications;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Orders;
using SokoHub.Domain.Modules.Payments;

namespace SokoHub.Application.Payments;

public record HandleMpesaCallbackCommand(
    string CheckoutRequestId,
    int ResultCode,
    string ResultDescription,
    string? MpesaReceiptNumber,
    decimal? Amount) : IRequest<Result<bool>>;

public sealed class HandleMpesaCallbackHandler : IRequestHandler<HandleMpesaCallbackCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<HandleMpesaCallbackHandler> _logger;

    public HandleMpesaCallbackHandler(
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        IEventBus eventBus,
        ILogger<HandleMpesaCallbackHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(HandleMpesaCallbackCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing M-Pesa Callback for CheckoutRequestId: {CheckoutRequestId}", request.CheckoutRequestId);

        // 1. Idempotency Check using Redis & MpesaReceiptNumber
        if (!string.IsNullOrWhiteSpace(request.MpesaReceiptNumber))
        {
            var processedKey = $"mpesa:receipt:{request.MpesaReceiptNumber}";
            if (await _cacheService.ExistsAsync(processedKey, cancellationToken))
            {
                _logger.LogWarning("Duplicate M-Pesa callback received for receipt: {ReceiptNumber}", request.MpesaReceiptNumber);
                return Result<bool>.Success(true);
            }
        }

        // 2. Resolve Payment via CheckoutRequestId from Cache or Spec
        var paymentId = await _cacheService.GetAsync<Guid>($"mpesa:checkout:{request.CheckoutRequestId}", cancellationToken);
        Payment? payment = null;

        if (paymentId != Guid.Empty)
        {
            payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(paymentId, cancellationToken);
        }

        if (payment == null)
        {
            _logger.LogError("Payment with CheckoutRequestId {CheckoutRequestId} not found.", request.CheckoutRequestId);
            return Result<bool>.Failure(new ApplicationError("payment_not_found", "Payment not found for the given checkout request id."));
        }

        Money? paidAmount = request.Amount.HasValue
            ? Money.Create(request.Amount.Value, payment.Amount.Currency)
            : null;

        payment.ApplyCallback(
            request.CheckoutRequestId,
            request.ResultCode,
            request.ResultDescription,
            request.MpesaReceiptNumber,
            paidAmount);

        if (payment.Status == PaymentStatus.Succeeded)
        {
            // Update order status
            var order = await _unitOfWork.Repository<Order>().GetByIdAsync(payment.OrderId, cancellationToken);
            if (order != null)
            {
                order.ConfirmPayment(payment.Reference);
                await _eventBus.PublishAsync(new OrderConfirmedIntegrationEvent(
                    order.Id,
                    order.Number.Value,
                    order.CustomerId,
                    order.GrandTotal.Amount,
                    payment.Reference.Value), cancellationToken);
            }

            // Mark receipt as processed in cache with 24-hour TTL for idempotency
            if (!string.IsNullOrWhiteSpace(request.MpesaReceiptNumber))
            {
                await _cacheService.SetAsync(
                    $"mpesa:receipt:{request.MpesaReceiptNumber}",
                    payment.Id,
                    TimeSpan.FromHours(24),
                    cancellationToken);
            }

            await _eventBus.PublishAsync(new PaymentSucceededIntegrationEvent(
                payment.Id,
                payment.OrderId,
                payment.Amount.Amount,
                payment.Amount.Currency,
                payment.Method.ToString(),
                payment.Reference.Value,
                request.MpesaReceiptNumber), cancellationToken);
        }
        else
        {
            await _eventBus.PublishAsync(new PaymentFailedIntegrationEvent(
                payment.Id,
                payment.OrderId,
                payment.Amount.Amount,
                request.ResultDescription,
                request.ResultCode.ToString()), cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }
}
