using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MarketplaceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceService.Infrastructure.Repositories;

public class SellerReviewRepository : ISellerReviewRepository
{
    private readonly AppDbContext _context;

    public SellerReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SellerReview?> GetByOrderIdAsync(Guid orderId)
        => await _context.SellerReviews.FirstOrDefaultAsync(r => r.OrderId == orderId);

    public async Task<List<SellerReview>> GetBySellerIdAsync(Guid sellerId)
        => await _context.SellerReviews
            .Where(r => r.SellerId == sellerId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(SellerReview review)
    {
        await _context.SellerReviews.AddAsync(review);
        await _context.SaveChangesAsync();
    }
}