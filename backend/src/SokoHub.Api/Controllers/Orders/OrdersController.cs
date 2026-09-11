using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SokoHub.Application.Modules.Orders;
using SokoHub.Application.Modules.Orders.Queries;

namespace SokoHub.Api.Controllers.Orders;

[ApiController]
[Route("api/orders")]
[Authorize]
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
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id)
    {
        var result = await _sender.Send(new GetOrderQuery(id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetCustomerOrders(Guid customerId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _sender.Send(new GetCustomerOrdersQuery(customerId, page, pageSize));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("vendor/{vendorId:guid}")]
    public async Task<IActionResult> GetVendorOrders(Guid vendorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _sender.Send(new GetVendorOrdersQuery(vendorId, page, pageSize));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var result = await _sender.Send(new ConfirmOrderCommand(id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, [FromQuery] string reason = "cancelled_by_customer")
    {
        var result = await _sender.Send(new CancelOrderCommand(id, reason));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/ship")]
    public async Task<IActionResult> Ship(Guid id, [FromBody] ShipOrderRequest request)
    {
        var result = await _sender.Send(new ShipOrderCommand(id, request.VendorOrderId, request.TrackingNumber));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> Deliver(Guid id, [FromBody] DeliverOrderRequest request)
    {
        var result = await _sender.Send(new DeliverOrderCommand(id, request.VendorOrderId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/pack")]
    [Authorize(Policy = "VendorOnly")]
    public async Task<IActionResult> Pack(Guid id, [FromBody] PackOrderRequest request)
    {
        var result = await _sender.Send(new PackOrderCommand(request.VendorOrderId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}

public record ShipOrderRequest(Guid VendorOrderId, string TrackingNumber);

public record DeliverOrderRequest(Guid VendorOrderId);

public record PackOrderRequest(Guid VendorOrderId);
