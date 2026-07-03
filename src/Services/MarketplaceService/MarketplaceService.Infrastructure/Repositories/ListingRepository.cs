using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MarketplaceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceService.Infrastructure.Repositories;

public class ListingRepository : IListingRepository
{
    private readonly AppDbContext _context;

    public ListingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Listing?> GetByIdAsync(Guid id)
        => await _context.Listings
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<List<Listing>> GetAllActiveAsync()
        => await _context.Listings
            .Where(l => l.Status == ListingStatus.Active)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<List<Listing>> GetBySellerIdAsync(Guid sellerId)
        => await _context.Listings
            .Where(l => l.SellerId == sellerId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Listing listing)
    {
        await _context.Listings.AddAsync(listing);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Listing listing)
    {
        await _context.SaveChangesAsync();
    }
}