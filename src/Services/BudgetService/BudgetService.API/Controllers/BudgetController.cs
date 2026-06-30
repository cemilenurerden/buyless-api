using System.Security.Claims;
using BudgetService.Application.Commands;
using BudgetService.Application.Queries;
using BudgetService.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Bu controller'daki tüm endpoint'ler geçerli bir JWT gerektirir
public class BudgetController : ControllerBase
{
    private readonly IMediator _mediator;

    public BudgetController(IMediator mediator)
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
    /// Giriş yapmış kullanıcının tüm bütçe hareketlerini (satış, bağış, manuel harcama) listeler,
    /// en son hareket en üstte. Opsiyonel "type" query parametresiyle (Sale/Donation/ManualExpense)
    /// filtrelenebilir.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ActivityType? type)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var activities = await _mediator.Send(new GetUserActivitiesQuery { UserId = userId, Type = type });
        return Ok(activities);
    }

    /// <summary>
    /// Belirtilen kategoriye ait tüm hareketleri listeler.
    /// </summary>
    [HttpGet("by-category/{categoryId}")]
    public async Task<IActionResult> GetByCategory(int categoryId)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var activities = await _mediator.Send(new GetActivitiesByCategoryQuery { UserId = userId, CategoryId = categoryId });
        return Ok(activities);
    }

    /// <summary>
    /// Tüm zamanların özetini döndürür (hesap açıldığından bu yana). Aylık özetle aynı
    /// alanları içerir, sadece bir zaman aralığıyla sınırlı değildir.
    /// </summary>
    [HttpGet("stats/lifetime")]
    public async Task<IActionResult> GetLifetimeStats()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var stats = await _mediator.Send(new GetLifetimeStatsQuery { UserId = userId });
        return Ok(stats);
    }

    /// <summary>
    /// Belirtilen ay için özet istatistik döndürür: toplam satış/harcama tutarı, net hareket,
    /// hareket sayıları, en sık kullanılan platform ve kategori bazlı kırılım.
    /// Kategori isimleri InventoryService'ten çekilir; o servise ulaşılamazsa
    /// categoryName alanları null döner ama özetin geri kalanı yine de hesaplanır.
    /// </summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] int year, [FromQuery] int month)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var summary = await _mediator.Send(new GetBudgetSummaryQuery
        {
            UserId = userId,
            Year = year,
            Month = month
        });

        return Ok(summary);
    }

    /// <summary>
    /// Kullanıcının elle girdiği bir harcamayı kaydeder (örn. "bu kazak için 250 TL verdim").
    /// İsteğe bağlı olarak bir InventoryService eşyasına (ItemId) bağlanabilir.
    /// </summary>
    [HttpPost("expenses")]
    public async Task<IActionResult> CreateManualExpense([FromBody] CreateManualExpenseCommand command)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.UserId = userId;

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    /// <summary>
    /// Kullanıcının elle girdiği bir harcamayı siler. Sadece ManualExpense türündeki kayıtlar
    /// silinebilir; Sale/Donation kayıtları InventoryService'ten otomatik geldiği için
    /// burada silinemez.
    /// </summary>
    [HttpDelete("expenses/{id}")]
    public async Task<IActionResult> DeleteManualExpense(Guid id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        await _mediator.Send(new DeleteManualExpenseCommand { Id = id, UserId = userId });
        return NoContent();
    }

    /// <summary>
    /// InventoryService'ten otomatik çağrılır: bir eşya satıldığında bütçe hareketi olarak kaydedilir.
    /// Bu endpoint, isteği gönderen kullanıcının kendi UserId'sini kullanır (JWT'den okunur);
    /// InventoryService, satışı yapan kullanıcının token'ını ileterek bu isteği atar.
    /// </summary>
    [HttpPost("activities/sale")]
    public async Task<IActionResult> RecordSale([FromBody] RecordSaleCommand command)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.UserId = userId;

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    /// <summary>
    /// InventoryService'ten otomatik çağrılır: bir eşya bağışlandığında bütçe hareketi olarak kaydedilir.
    /// </summary>
    [HttpPost("activities/donation")]
    public async Task<IActionResult> RecordDonation([FromBody] RecordDonationCommand command)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        command.UserId = userId;

        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }
}