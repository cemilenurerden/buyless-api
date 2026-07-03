using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MarketplaceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceService.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id)
        => await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);

    public async Task<List<Order>> GetByBuyerIdAsync(Guid buyerId)
        => await _context.Orders
            .Where(o => o.BuyerId == buyerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<List<Order>> GetBySellerIdAsync(Guid sellerId)
        => await _context.Orders
            .Where(o => o.SellerId == sellerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        await _context.SaveChangesAsync();
    }
}