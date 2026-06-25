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

    /// <summary>
    /// Giriş yapmış kullanıcının envanterine yeni bir eşya ekler.
    /// UserId, client'tan alınmaz; JWT token içindeki kullanıcı kimliğinden otomatik okunur.
    /// </summary>
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

    /// <summary>
    /// Belirtilen eşya için bir giyilme kaydı (wear log) oluşturur ve eşyanın giyilme sayacını günceller.
    /// Eşya, isteği gönderen kullanıcıya ait olmalıdır; aksi halde reddedilir.
    /// </summary>
    [HttpPost("{itemId}/wear")]
    public async Task<IActionResult> RecordWear(Guid itemId, [FromBody] RecordWearRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue("sub");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var command = new RecordWearCommand
        {
            ItemId = itemId,
            UserId = userId,
            WornDate = request.WornDate
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

/// <summary>
/// RecordWear endpoint'i için istek gövdesi. Sadece giyilme tarihini içerir;
/// ItemId route'tan, UserId ise JWT token'dan otomatik alınır.
/// </summary>
public class RecordWearRequest
{
    public DateTime WornDate { get; set; }
}