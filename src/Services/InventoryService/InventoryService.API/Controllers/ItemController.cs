using System.Security.Claims;
using InventoryService.Application.Commands;
using InventoryService.Application.Queries;
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

    private bool TryGetUserId(out Guid userId)
    {
        userId = Guid.Empty;

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User.FindFirstValue("sub");

        if (userIdClaim is null)
            return false;

        return Guid.TryParse(userIdClaim, out userId);
    }

    /// <summary>
    /// Giriş yapmış kullanıcının envanterine yeni bir eşya ekler.
    /// UserId, client'tan alınmaz; JWT token içindeki kullanıcı kimliğinden otomatik okunur.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] AddItemCommand command)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.UserId = userId;

        var itemId = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = itemId }, new { id = itemId });
    }

    /// <summary>
    /// Giriş yapmış kullanıcının tüm eşyalarını listeler (en yeni eklenen en üstte).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var items = await _mediator.Send(new GetUserItemsQuery { UserId = userId });
        return Ok(items);
    }

    /// <summary>
    /// Tek bir eşyayı, verilen ID'ye göre getirir. Eşya, isteği gönderen kullanıcıya ait olmalıdır.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var item = await _mediator.Send(new GetItemByIdQuery { Id = id, UserId = userId });

        if (item is null)
            return NotFound(new { message = "Belirtilen eşya bulunamadı." });

        return Ok(item);
    }

    /// <summary>
    /// Var olan bir eşyanın adını, rengini, bedenini, resmini, durumunu (Condition) veya notlarını günceller.
    /// Eşya, isteği gönderen kullanıcıya ait olmalıdır.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateItemCommand command)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.Id = id;
        command.UserId = userId;

        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Belirtilen eşya için bir giyilme kaydı (wear log) oluşturur ve eşyanın giyilme sayacını günceller.
    /// Eşya, isteği gönderen kullanıcıya ait olmalıdır; aksi halde reddedilir.
    /// </summary>
    [HttpPost("{itemId}/wear")]
    public async Task<IActionResult> RecordWear(Guid itemId, [FromBody] RecordWearRequest request)
    {
        if (!TryGetUserId(out var userId))
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

    /// <summary>
    /// Eşyayı "Donated" (bağışlandı) durumuna geçirir. Eşya zaten Active değilse reddedilir.
    /// Bu işlem, BudgetService'e otomatik olarak bir bağış kaydı bildirir.
    /// </summary>
    [HttpPost("{id}/donate")]
    public async Task<IActionResult> Donate(Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var accessToken = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        await _mediator.Send(new DonateItemCommand { Id = id, UserId = userId, AccessToken = accessToken });
        return NoContent();
    }

    /// <summary>
    /// Eşyayı "Sold" (satıldı) durumuna geçirir. Eşya zaten Active değilse reddedilir.
    /// Bu işlem, BudgetService'e otomatik olarak bir satış kaydı bildirir.
    /// </summary>
    [HttpPost("{id}/sell")]
    public async Task<IActionResult> Sell(Guid id, [FromBody] SellItemRequest request)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var accessToken = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        await _mediator.Send(new SellItemCommand
        {
            Id = id,
            UserId = userId,
            Amount = request.Amount,
            AccessToken = accessToken
        });
        return NoContent();
    }

    /// <summary>
    /// Eşyayı "Recycled" (geri dönüştürüldü) durumuna geçirir. Eşya zaten Active değilse reddedilir.
    /// </summary>
    [HttpPost("{id}/recycle")]
    public async Task<IActionResult> Recycle(Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        await _mediator.Send(new RecycleItemCommand { Id = id, UserId = userId });
        return NoContent();
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

/// <summary>
/// Sell endpoint'i için istek gövdesi. Satış tutarı, BudgetService'e bildirim
/// gönderirken kullanılır.
/// </summary>
public class SellItemRequest
{
    public decimal Amount { get; set; }
}