using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using MediatR;

namespace InventoryService.Application.Queries;

public class GetBarcodeProductQueryHandler : IRequestHandler<GetBarcodeProductQuery, BarcodeProductCache?>
{
    private readonly IBarcodeProductCacheRepository _cacheRepository;

    public GetBarcodeProductQueryHandler(IBarcodeProductCacheRepository cacheRepository)
    {
        _cacheRepository = cacheRepository;
    }

    public async Task<BarcodeProductCache?> Handle(GetBarcodeProductQuery request, CancellationToken cancellationToken)
    {
        // Şimdilik sadece kendi cache'imize bakıyoruz.
        // İleride: cache'te bulunamazsa, burada dış bir barkod/ürün API'sine istek atılacak,
        // sonuç hem kullanıcıya döndürülecek hem de _cacheRepository.AddAsync ile cache'e yazılacak.
        return await _cacheRepository.GetByBarcodeAsync(request.Barcode);
    }
}