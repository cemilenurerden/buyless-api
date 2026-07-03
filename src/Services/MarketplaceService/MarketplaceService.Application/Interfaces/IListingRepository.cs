using MarketplaceService.Domain;

namespace MarketplaceService.Application.Interfaces;

public interface IListingRepository
{
    Task<Listing?> GetByIdAsync(Guid id);
    Task<List<Listing>> GetAllActiveAsync();
    Task<List<Listing>> GetBySellerIdAsync(Guid sellerId);
    Task AddAsync(Listing listing);
    Task UpdateAsync(Listing listing);
}