using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SokoHub.Application.Modules.Payments.Queries;
using SokoHub.Application.Payments;
using SokoHub.Domain.Common.ValueObjects;
using SokoHub.Contracts.Payments;

namespace SokoHub.Api.Controllers.Payments;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly ISender _sender;

    public PaymentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("initiate")]
    [Authorize]
    public async Task<IActionResult> Initiate([FromBody] PaymentInitiateRequest request)
    {
        var command = new InitiatePaymentCommand(
            request.OrderId,
            request.PhoneNumber,
            request.Amount,
            request.PaymentMethod,
            request.Currency);

        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/refund")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Refund(Guid id, [FromBody] PaymentRefundRequest request)
    {
        var command = new RefundPaymentCommand(id, Money.Create(request.Amount, request.Currency), request.Reason ?? "manual_refund");
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var response = await _sender.Send(new GetPaymentByIdQuery(id));
        return Ok(response);
    }
}
