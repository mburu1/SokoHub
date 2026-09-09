using Microsoft.AspNetCore.Mvc;
using MediatR;
using SokoHub.Application.Modules.Cart;
using SokoHub.Application.Modules.Cart.Queries;

namespace SokoHub.Api.Controllers.Cart;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ISender _sender;

    public CartController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var result = await _sender.Send(new GetCartQuery());
        return Ok(result);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpDelete("items/{productId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid productId)
    {
        await _sender.Send(new RemoveCartItemCommand(productId));
        return NoContent();
    }

    [HttpPut("items")]
    public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemCommand command)
    {
        var result = await _sender.Send(command);
        return Ok(result);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        await _sender.Send(new ClearCartCommand());
        return NoContent();
    }
}
