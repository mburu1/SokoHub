using Microsoft.AspNetCore.Mvc;
using MediatR;
using SokoHub.Application.Modules.Customers;
using SokoHub.Application.Modules.Customers.Queries;

namespace SokoHub.Api.Controllers.Customers;

[ApiController]
[Route("api/customers")]
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
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command)
    {
        var result = await _sender.Send(command with { Id = id });
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        var result = await _sender.Send(new GetByIdQuery(id));
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] GetListQuery query)
    {
        var result = await _sender.Send(query);
        return Ok(result);
    }

    [HttpPost("{id:guid}/addresses")]
    public async Task<IActionResult> AddAddress(Guid id, [FromBody] AddAddressCommand command)
    {
        var result = await _sender.Send(command with { CustomerId = id });
        return Ok(result);
    }

    [HttpDelete("{id:guid}/addresses/{addressId:guid}")]
    public async Task<IActionResult> RemoveAddress(Guid id, Guid addressId)
    {
        await _sender.Send(new RemoveAddressCommand(id, addressId));
        return NoContent();
    }

    [HttpPut("{id:guid}/addresses/{addressId:guid}")]
    public async Task<IActionResult> UpdateAddress(Guid id, Guid addressId, [FromBody] UpdateAddressCommand command)
    {
        var result = await _sender.Send(command with { CustomerId = id, AddressId = addressId });
        return Ok(result);
    }
}
