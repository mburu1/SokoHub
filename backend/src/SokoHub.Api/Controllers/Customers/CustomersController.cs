using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SokoHub.Application.Modules.Customers;
using SokoHub.Application.Modules.Customers.Queries;

namespace SokoHub.Api.Controllers.Customers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly ISender _sender;

    public CustomersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command)
    {
        var result = await _sender.Send(command with { Id = id });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        var result = await _sender.Send(new GetByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] GetListQuery query)
    {
        var result = await _sender.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("{id:guid}/addresses")]
    public async Task<IActionResult> AddAddress(Guid id, [FromBody] AddAddressCommand command)
    {
        var result = await _sender.Send(command with { CustomerId = id });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}/addresses/{addressId:guid}")]
    public async Task<IActionResult> RemoveAddress(Guid id, Guid addressId)
    {
        var result = await _sender.Send(new RemoveAddressCommand(id, addressId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id:guid}/addresses/{addressId:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid id, Guid addressId, [FromBody] UpdateAddressCommand command)
    {
        var result = await _sender.Send(command with { CustomerId = id, AddressId = addressId });
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
