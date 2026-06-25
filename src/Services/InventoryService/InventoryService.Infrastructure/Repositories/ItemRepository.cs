using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using InventoryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly AppDbContext _context;

    public ItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Item?> GetByIdAsync(Guid id)
        => await _context.Items.FirstOrDefaultAsync(i => i.Id == id);

    public async Task AddAsync(Item item)
    {
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Item item)
    {
        // Item, GetByIdAsync ile çekildiği için zaten EF Core change tracker tarafından takip ediliyor.
        // RecordWear() gibi domain metotları entity üzerinde değişiklik yaptığında,
        // burada sadece SaveChangesAsync çağırmak EF Core'un bu değişiklikleri algılayıp
        // gerekli UPDATE sorgusunu üretmesi için yeterli.
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByCategoryIdAsync(int categoryId)
        => await _context.Items.AnyAsync(i => i.CategoryId == categoryId);

    public async Task<bool> ExistsByBrandIdAsync(int brandId)
        => await _context.Items.AnyAsync(i => i.BrandId == brandId);
}