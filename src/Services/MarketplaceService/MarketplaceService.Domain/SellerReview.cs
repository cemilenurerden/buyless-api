namespace MarketplaceService.Domain;

public class SellerReview
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ReviewerId { get; private set; }   // Alıcı
    public Guid SellerId { get; private set; }     // Satıcı
    public int Rating { get; private set; }        // 1-5
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public Order Order { get; private set; } = null!;

    private SellerReview() { }

    public SellerReview(Guid orderId, Guid reviewerId, Guid sellerId, int rating, string? comment = null)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentException("Puan 1 ile 5 arasında olmalıdır.", nameof(rating));

        Id = Guid.NewGuid();
        OrderId = orderId;
        ReviewerId = reviewerId;
        SellerId = sellerId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }
}