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
public class ListingController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICloudinaryService _cloudinaryService;

    public ListingController(IMediator mediator, ICloudinaryService cloudinaryService)
    {
        _mediator = mediator;
        _cloudinaryService = cloudinaryService;
    }

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return claim is not null && Guid.TryParse(claim, out userId);
    }

    /// <summary>
    /// Tüm aktif ilanları listeler. Herkese açıktır.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var listings = await _mediator.Send(new GetAllListingsQuery());
        return Ok(listings);
    }

    /// <summary>
    /// Tek bir ilanı getirir. Herkese açıktır. Görüntülenme sayısını artırır.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var listing = await _mediator.Send(new GetListingByIdQuery { Id = id });

        if (listing is null)
            return NotFound(new { message = "Belirtilen ilan bulunamadı." });

        return Ok(listing);
    }

    /// <summary>
    /// Giriş yapmış kullanıcının kendi ilanlarını listeler.
    /// </summary>
    [Authorize]
    [HttpGet("my")]
    public async Task<IActionResult> GetMyListings()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var listings = await _mediator.Send(new GetMyListingsQuery { SellerId = userId });
        return Ok(listings);
    }

    /// <summary>
    /// Yeni bir ilan oluşturur. Giriş yapmış kullanıcı gereklidir.
    /// ItemId opsiyoneldir — dolaptaki mevcut bir eşyaya bağlanabilir.
    /// </summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateListingCommand command)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.SellerId = userId;

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Bir ilana fotoğraf yükler. Cloudinary'ye yüklenir, URL veritabanına kaydedilir.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/images")]
    public async Task<IActionResult> UploadImage(Guid id, IFormFile file, [FromQuery] int displayOrder = 0)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Geçerli bir dosya gönderilmedi." });

        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest(new { message = "Sadece JPEG, PNG veya WebP formatında fotoğraf yüklenebilir." });

        using var stream = file.OpenReadStream();
        var imageUrl = await _cloudinaryService.UploadImageAsync(stream, file.FileName);

        await _mediator.Send(new AddListingImageCommand
        {
            ListingId = id,
            SellerId = userId,
            ImageUrl = imageUrl,
            DisplayOrder = displayOrder
        });

        return Ok(new { imageUrl });
    }

    /// <summary>
    /// Bir ilanı satın alır. İlk "satın al" diyen kazanır.
    /// Kendi ilanınızı satın alamazsınız.
    /// </summary>
    [Authorize]
    [HttpPost("{id}/purchase")]
    public async Task<IActionResult> Purchase(Guid id, [FromBody] PurchaseRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var accessToken = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        var orderId = await _mediator.Send(new PurchaseListingCommand
        {
            ListingId = id,
            BuyerId = userId,
            ShippingAddress = request.ShippingAddress,
            AccessToken = accessToken
        });

        return Ok(new { orderId });
    }
}

public class PurchaseRequest
{
    public string? ShippingAddress { get; set; }
}