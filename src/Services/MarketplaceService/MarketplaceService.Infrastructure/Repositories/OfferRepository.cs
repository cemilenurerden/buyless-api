using MarketplaceService.Application.Interfaces;
using MarketplaceService.Domain;
using MarketplaceService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MarketplaceService.Infrastructure.Repositories;

public class OfferRepository : IOfferRepository
{
    private readonly AppDbContext _context;

    public OfferRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Offer?> GetByIdAsync(Guid id)
        => await _context.Offers.FirstOrDefaultAsync(o => o.Id == id);

    public async Task<List<Offer>> GetByListingIdAsync(Guid listingId)
        => await _context.Offers
            .Where(o => o.ListingId == listingId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task<List<Offer>> GetByBuyerIdAsync(Guid buyerId)
        => await _context.Offers
            .Where(o => o.BuyerId == buyerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

    public async Task AddAsync(Offer offer)
    {
        await _context.Offers.AddAsync(offer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Offer offer)
    {
        await _context.SaveChangesAsync();
    }
}