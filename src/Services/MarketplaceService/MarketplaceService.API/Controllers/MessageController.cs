using System.Security.Claims;
using MarketplaceService.Application.Commands;
using MarketplaceService.Application.Interfaces;
using MarketplaceService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MessageController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMessageRepository _messageRepository;

    public MessageController(IMediator mediator, IMessageRepository messageRepository)
    {
        _mediator = mediator;
        _messageRepository = messageRepository;
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return claim is not null && Guid.TryParse(claim, out userId);
    }

    /// <summary>
    /// Bir ilana ait mesajlaşmayı getirir. Sadece mesajlaşmanın tarafı olan kullanıcı görebilir.
    /// </summary>
    [HttpGet("listing/{listingId}")]
    public async Task<IActionResult> GetMessages(Guid listingId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var messages = await _mediator.Send(new GetMessagesQuery { ListingId = listingId, UserId = userId });
        return Ok(messages);
    }

    /// <summary>
    /// Bir ilana ait mesaj gönderir.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageCommand command)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.SenderId = userId;

        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    /// <summary>
    /// Bir ilana ait, kullanıcıya gelen okunmamış mesajları okundu olarak işaretler.
    /// </summary>
    [HttpPost("listing/{listingId}/mark-read")]
    public async Task<IActionResult> MarkAsRead(Guid listingId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        await _messageRepository.MarkAsReadAsync(listingId, userId);
        return NoContent();
    }
}