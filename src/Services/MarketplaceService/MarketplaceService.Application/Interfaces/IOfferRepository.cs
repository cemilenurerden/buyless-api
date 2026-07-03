using MarketplaceService.Domain;

namespace MarketplaceService.Application.Interfaces;

public interface IOfferRepository
{
    Task<Offer?> GetByIdAsync(Guid id);
    Task<List<Offer>> GetByListingIdAsync(Guid listingId);
    Task<List<Offer>> GetByBuyerIdAsync(Guid buyerId);
    Task AddAsync(Offer offer);
    Task UpdateAsync(Offer offer);
}