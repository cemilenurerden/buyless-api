using InventoryService.Domain;

namespace InventoryService.Application.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id);
    Task AddAsync(Item item);
}