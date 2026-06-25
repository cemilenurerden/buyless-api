using InventoryService.Application.Commands;
using InventoryService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm markaları listeler. Herkese açıktır (giriş yapmış olmana gerek yok).
    /// Mobil uygulamada "marka seç" dropdown'unu doldurmak için kullanılır.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var brands = await _mediator.Send(new GetAllBrandsQuery());
        return Ok(brands);
    }

    /// <summary>
    /// Tek bir markayı, verilen ID'ye göre getirir. Herkese açıktır.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var brand = await _mediator.Send(new GetBrandByIdQuery { Id = id });

        if (brand is null)
            return NotFound(new { message = "Belirtilen marka bulunamadı." });

        return Ok(brand);
    }

    /// <summary>
    /// Yeni bir marka oluşturur. Sadece Admin rolündeki kullanıcılar kullanabilir.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBrandCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Var olan bir markanın adını/logosunu günceller. Sadece Admin rolündeki kullanıcılar kullanabilir.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Bir markayı siler. Sadece Admin rolündeki kullanıcılar kullanabilir.
    /// Marka herhangi bir eşyada kullanılıyorsa silinemez.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteBrandCommand { Id = id });
        return NoContent();
    }
}