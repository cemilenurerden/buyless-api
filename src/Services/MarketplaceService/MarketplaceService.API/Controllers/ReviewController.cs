using System.Security.Claims;
using MarketplaceService.Application.Commands;
using MarketplaceService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketplaceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewController(IMediator mediator)
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
    /// Bir satıcının tüm değerlendirmelerini listeler. Herkese açıktır.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("seller/{sellerId}")]
    public async Task<IActionResult> GetSellerReviews(Guid sellerId)
    {
        var reviews = await _mediator.Send(new GetSellerReviewsQuery { SellerId = sellerId });
        return Ok(reviews);
    }

    /// <summary>
    /// Teslim edilmiş bir sipariş için satıcıyı değerlendirir (1-5 puan).
    /// Sadece alıcı değerlendirme yapabilir ve her sipariş için yalnızca bir kez yapılabilir.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateSellerReviewCommand command)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.ReviewerId = userId;

        var id = await _mediator.Send(command);
        return Ok(new { id });
    }
}