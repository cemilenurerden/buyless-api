using System.Security.Claims;
using InventoryService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Bu controller'daki tüm endpoint'ler geçerli bir JWT gerektirir
public class ItemController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] AddItemCommand command)
    {
        // UserId, client'tan ASLA güvenilir bilgi olarak alınmaz.
        // JWT token imzalı olduğu için içindeki Sub/NameIdentifier claim'i taklit edilemez -
        // bu yüzden UserId'yi buradan okuyup Command'e biz atıyoruz.
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue("sub");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.UserId = userId;

        var itemId = await _mediator.Send(command);

        return CreatedAtAction(nameof(AddItem), new { id = itemId }, new { id = itemId });
    }
}