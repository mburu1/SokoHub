using Microsoft.AspNetCore.Mvc;
using MediatR;
using SokoHub.Application.Modules.Finance;
using SokoHub.Application.Modules.Finance.Queries;

namespace SokoHub.Api.Controllers.Finance;

[ApiController]
[Route("api/finance")]
public class FinanceController : ControllerBase
{
    private readonly ISender _sender;

    public FinanceController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("journal-entry")]
    public async Task<IActionResult> CreateJournalEntry([FromBody] CreateJournalEntryCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPost("settlement")]
    public async Task<IActionResult> CreateSettlement([FromBody] CreateSettlementCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpPost("payout")]
    public async Task<IActionResult> ProcessPayout([FromBody] ProcessVendorPayoutCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpGet("statement/{vendorId:guid}")]
    public async Task<IActionResult> GetVendorStatement(Guid vendorId)
    {
        var result = await _sender.Send(new GetVendorStatementQuery(vendorId));
        return Ok(result);
    }

    [HttpPost("reconcile-settlement")]
    public async Task<IActionResult> ReconcileSettlement([FromBody] ReconcileSettlementCommand command)
    {
        await _sender.Send(command);
        return Ok();
    }
}
