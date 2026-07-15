using MarketplaceService.Domain;

namespace MarketplaceService.Application.Interfaces;

public interface ISellerReviewRepository
{
    Task<SellerReview?> GetByOrderIdAsync(Guid orderId);
    Task<List<SellerReview>> GetBySellerIdAsync(Guid sellerId);
    Task AddAsync(SellerReview review);
}

public interface IMessageRepository
{
    Task<List<Message>> GetByListingIdAsync(Guid listingId, Guid userId);
    Task AddAsync(Message message);
    Task MarkAsReadAsync(Guid listingId, Guid receiverId);
}