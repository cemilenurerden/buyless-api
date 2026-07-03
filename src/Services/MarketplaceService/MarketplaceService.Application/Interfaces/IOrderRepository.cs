using MarketplaceService.Domain;

namespace MarketplaceService.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<List<Order>> GetByBuyerIdAsync(Guid buyerId);
    Task<List<Order>> GetBySellerIdAsync(Guid sellerId);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}