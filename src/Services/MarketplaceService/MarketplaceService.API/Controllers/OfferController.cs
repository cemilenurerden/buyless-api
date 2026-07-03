using System.Security.Claims;
using MarketplaceService.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OfferController : ControllerBase
{
    private readonly IMediator _mediator;

    public OfferController(IMediator mediator)
    {
        _mediator = mediator;
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return claim is not null && Guid.TryParse(claim, out userId);
    }

    /// <summary>
    /// Bir ilana teklif verir. Kendi ilanınıza teklif veremezsiniz.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> MakeOffer([FromBody] MakeOfferCommand command)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.BuyerId = userId;

        var id = await _mediator.Send(command);
        return Ok(new { id });
    }

    /// <summary>
    /// Satıcı, kendine gelen bir teklifi kabul veya reddeder.
    /// Kabul edilirse otomatik olarak sipariş oluşturulur ve ilan satıldı olarak işaretlenir.
    /// </summary>
    [HttpPost("{offerId}/respond")]
    public async Task<IActionResult> Respond(Guid offerId, [FromBody] RespondToOfferRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var accessToken = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        var orderId = await _mediator.Send(new RespondToOfferCommand
        {
            OfferId = offerId,
            SellerId = userId,
            Accept = request.Accept,
            AccessToken = accessToken
        });

        if (orderId.HasValue)
            return Ok(new { orderId });

        return Ok(new { message = "Teklif reddedildi." });
    }
}

public class RespondToOfferRequest
{
    public bool Accept { get; set; }
}