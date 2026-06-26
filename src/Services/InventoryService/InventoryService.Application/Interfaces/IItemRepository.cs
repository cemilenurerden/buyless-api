using InventoryService.Domain;

namespace InventoryService.Application.Interfaces;

public interface IItemRepository
{
    Task<Item?> GetByIdAsync(Guid id);
    Task<List<Item>> GetByUserIdAsync(Guid userId);
    Task AddAsync(Item item);
    Task UpdateAsync(Item item);
    Task<bool> ExistsByCategoryIdAsync(int categoryId);
    Task<bool> ExistsByBrandIdAsync(int brandId);
}