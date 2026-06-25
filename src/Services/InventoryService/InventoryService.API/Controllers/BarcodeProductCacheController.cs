using InventoryService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BarcodeProductCacheController : ControllerBase
{
    private readonly IMediator _mediator;

    public BarcodeProductCacheController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Verilen barkoda ait ürün bilgisini cache'ten getirir. Herkese açıktır.
    /// Şu an sadece kendi cache'imize bakılıyor; cache'te yoksa 404 döner.
    /// İleride, cache'te bulunamayan barkodlar için dış bir ürün/barkod API'sine
    /// otomatik sorgu atılması ve sonucun cache'e yazılması planlanıyor.
    /// </summary>
    [HttpGet("{barcode}")]
    public async Task<IActionResult> GetByBarcode(string barcode)
    {
        var result = await _mediator.Send(new GetBarcodeProductQuery { Barcode = barcode });

        if (result is null)
            return NotFound(new { message = "Bu barkoda ait ürün bilgisi bulunamadı." });

        return Ok(result);
    }
}