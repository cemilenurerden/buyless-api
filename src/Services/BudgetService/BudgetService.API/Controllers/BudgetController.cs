using System.Security.Claims;
using BudgetService.Application.Commands;
using BudgetService.Application.Queries;
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
    /// en son hareket en üstte.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized(new { message = "Token içinde geçerli bir kullanıcı kimliği bulunamadı." });

        var activities = await _mediator.Send(new GetUserActivitiesQuery { UserId = userId });
        return Ok(activities);
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
