using InventoryService.Domain;

namespace InventoryService.Application.Interfaces;

public interface IItemWearLogRepository
{
    Task AddAsync(ItemWearLog wearLog);
}
