using InventoryService.Domain;

namespace InventoryService.Application.Interfaces;

public interface IBarcodeProductCacheRepository
{
    Task<BarcodeProductCache?> GetByBarcodeAsync(string barcode);
    Task AddAsync(BarcodeProductCache cacheEntry);
    Task UpdateAsync(BarcodeProductCache cacheEntry);
}