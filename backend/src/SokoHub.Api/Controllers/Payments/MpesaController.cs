using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SokoHub.Application.Payments;
using SokoHub.Contracts.Payments;

namespace SokoHub.Api.Controllers.Payments;

[ApiController]
[Route("api/payments/mpesa")]
public class MpesaController : ControllerBase
{
    private readonly ISender _sender;

    public MpesaController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("stkpush")]
    [Authorize]
    public async Task<IActionResult> InitiateStkPush([FromBody] InitiatePaymentCommand command)
    {
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> StkCallback([FromBody] DarajaCallbackPayload payload)
    {
        var stkCallback = payload.Body.StkCallback;

        var amount = stkCallback.CallbackMetadata?.Item
            .FirstOrDefault(i => i.Name == "Amount")
            .Value?.ToString();

        var receipt = stkCallback.CallbackMetadata?.Item
            .FirstOrDefault(i => i.Name == "MpesaReceiptNumber")
            .Value?.ToString() ?? stkCallback.CheckoutRequestId;

        var command = new HandleMpesaCallbackCommand(
            stkCallback.CheckoutRequestId,
            stkCallback.ResultCode,
            stkCallback.ResultDesc,
            receipt,
            decimal.TryParse(amount, out var parsed) ? parsed : null);

        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("b2c/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> B2cCallback([FromBody] DarajaCallbackPayload payload)
    {
        // B2C result URL handler — persistence and async processing.
        return Ok(new { success = true });
    }
}
