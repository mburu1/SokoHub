using MediatR;
using SokoHub.Application.Common.Results;
using SokoHub.Application.Common.Errors;
using SokoHub.Domain.Interfaces;
using SokoHub.Domain.Modules.Payments;
using SokoHub.Application.Common.Interfaces;

namespace SokoHub.Application.Modules.Payments;

public record InitiateMpesaStkPushCommand(
    Guid PaymentId,
    string PhoneNumber,
    decimal Amount) : IRequest<Result<string>>;

public sealed class InitiateMpesaStkPushHandler : IRequestHandler<InitiateMpesaStkPushCommand, Result<string>>
{
    private readonly IUnitOfWork _unitOfWork;
    // In a real app, we'd inject an IMpesaClient here, but for now we use IUnitOfWork
    // if the client is registered in the infrastructure.

    public InitiateMpesaStkPushHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(InitiateMpesaStkPushCommand request, CancellationToken cancellationToken)
    {
        var payment = await _unitOfWork.Repository<Payment>().GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
        {
            return Result<string>.Failure(new ApplicationError("payment_not_found", "Payment not found."));
        }

        // Logic to trigger STK Push via Infrastructure (e.g. DarajaClient)
        // For now, we simulate the request.
        var checkoutRequestId = Guid.NewGuid().ToString();

        // Update payment status to Pending
        payment.UpdateStatus(PaymentStatus.Pending);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(checkoutRequestId);
    }
}
