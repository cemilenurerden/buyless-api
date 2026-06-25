using InventoryService.Application.Interfaces;
using InventoryService.Domain;
using InventoryService.Infrastructure.Persistence;

namespace InventoryService.Infrastructure.Repositories;

public class ItemWearLogRepository : IItemWearLogRepository
{
    private readonly AppDbContext _context;

    public ItemWearLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ItemWearLog wearLog)
    {
        await _context.ItemWearLogs.AddAsync(wearLog);
        await _context.SaveChangesAsync();
    }
}