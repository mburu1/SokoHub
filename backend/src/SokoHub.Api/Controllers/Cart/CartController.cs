using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SokoHub.Application.Modules.Cart;

namespace SokoHub.Api.Controllers.Cart;

[ApiController]
[Route("api/cart")]
[Authorize]
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
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemCommand command)
    {
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("items/{productVariantId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid productVariantId)
    {
        var result = await _sender.Send(new RemoveCartItemCommand(productVariantId));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("items")]
    public async Task<IActionResult> UpdateItem([FromBody] UpdateCartItemCommand command)
    {
        var result = await _sender.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> Clear()
    {
        var result = await _sender.Send(new ClearCartCommand());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}