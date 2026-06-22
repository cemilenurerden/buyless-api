using InventoryService.Domain;

namespace InventoryService.Application.Interfaces;

public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(int id);
    Task<List<Brand>> GetAllAsync();
    Task AddAsync(Brand brand);
    Task UpdateAsync(Brand brand);
    Task DeleteAsync(Brand brand);
}