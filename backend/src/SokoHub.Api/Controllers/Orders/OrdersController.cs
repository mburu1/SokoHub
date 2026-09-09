using Microsoft.AspNetCore.Mvc;
using MediatR;
using SokoHub.Application.Modules.Orders;
using SokoHub.Application.Modules.Orders.Queries;

namespace SokoHub.Api.Controllers.Orders;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var result = await _sender.Send(new GetOrderQuery(id));
        return Ok(result);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetCustomerOrders(Guid customerId)
    {
        var result = await _sender.Send(new GetCustomerOrdersQuery(customerId));
        return Ok(result);
    }

    [HttpGet("vendor/{vendorId:guid}")]
    public async Task<IActionResult> GetVendorOrders(Guid vendorId)
    {
        var result = await _sender.Send(new GetVendorOrdersQuery(vendorId));
        return Ok(result);
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        await _sender.Send(new ConfirmOrderCommand(id));
        return Ok();
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        await _sender.Send(new CancelOrderCommand(id));
        return Ok();
    }

    [HttpPost("{id:guid}/ship")]
    public async Task<IActionResult> Ship(Guid id)
    {
        await _sender.Send(new ShipOrderCommand(id));
        return Ok();
    }

    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> Deliver(Guid id)
    {
        await _sender.Send(new DeliverOrderCommand(id));
        return Ok();
    }

    [HttpPost("{id:guid}/pack")]
    public async Task<IActionResult> Pack(Guid id)
    {
        await _sender.Send(new PackOrderCommand(id));
        return Ok();
    }
}
