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
}