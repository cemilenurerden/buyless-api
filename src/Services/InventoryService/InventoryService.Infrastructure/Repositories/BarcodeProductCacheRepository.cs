using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using InventoryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public class BarcodeProductCacheRepository : IBarcodeProductCacheRepository
{
    private readonly AppDbContext _context;

    public BarcodeProductCacheRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BarcodeProductCache?> GetByBarcodeAsync(string barcode)
        => await _context.BarcodeProductCaches.FirstOrDefaultAsync(b => b.Barcode == barcode);

    public async Task AddAsync(BarcodeProductCache cacheEntry)
    {
        await _context.BarcodeProductCaches.AddAsync(cacheEntry);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(BarcodeProductCache cacheEntry)
    {
        _context.BarcodeProductCaches.Update(cacheEntry);
        await _context.SaveChangesAsync();
    }
}