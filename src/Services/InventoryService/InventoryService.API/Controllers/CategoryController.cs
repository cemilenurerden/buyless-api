using InventoryService.Application.Commands;
using InventoryService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Tüm kategorileri listeler. Herkese açıktır (giriş yapmış olmana gerek yok).
    /// Mobil uygulamada "kategori seç" dropdown'unu doldurmak için kullanılır.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _mediator.Send(new GetAllCategoriesQuery());
        return Ok(categories);
    }

    /// <summary>
    /// Tek bir kategoriyi, verilen ID'ye göre getirir. Herkese açıktır.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _mediator.Send(new GetCategoryByIdQuery { Id = id });

        if (category is null)
            return NotFound(new { message = "Belirtilen kategori bulunamadı." });

        return Ok(category);
    }

    /// <summary>
    /// Yeni bir kategori oluşturur. Sadece Admin rolündeki kullanıcılar kullanabilir.
    /// ParentCategoryId verilirse, üst kategorinin kendisi bir alt kategori olmamalıdır (2 seviye sınırı).
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Var olan bir kategorinin adını/ikon URL'ini günceller. Sadece Admin rolündeki kullanıcılar kullanabilir.
    /// (Üst kategori değişikliği bu endpoint'te desteklenmez.)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        await _mediator.Send(command);
        return NoContent();
    }

    /// <summary>
    /// Bir kategoriyi siler. Sadece Admin rolündeki kullanıcılar kullanabilir.
    /// Kategorinin alt kategorileri varsa veya kategoriye ait eşyalar varsa silinemez.
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _mediator.Send(new DeleteCategoryCommand { Id = id });
        return NoContent();
    }
}