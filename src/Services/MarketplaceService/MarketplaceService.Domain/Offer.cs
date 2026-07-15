namespace MarketplaceService.Domain;

public class Offer
{
    public Guid Id { get; private set; }
    public Guid ListingId { get; private set; }
    public Guid BuyerId { get; private set; }
    public decimal OfferedPrice { get; private set; }
    public string? Message { get; private set; }
    public OfferStatus Status { get; private set; } = OfferStatus.Pending;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Listing Listing { get; private set; } = null!;

    private Offer() { }

    public Offer(Guid listingId, Guid buyerId, decimal offeredPrice, string? message = null)
    {
        if (offeredPrice <= 0)
            throw new ArgumentException("Teklif fiyatı sıfırdan büyük olmalıdır.", nameof(offeredPrice));

        Id = Guid.NewGuid();
        ListingId = listingId;
        BuyerId = buyerId;
        OfferedPrice = offeredPrice;
        Message = message;
        Status = OfferStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Accept()
    {
        if (Status != OfferStatus.Pending)
            throw new InvalidOperationException("Sadece bekleyen teklifler kabul edilebilir.");

        Status = OfferStatus.Accepted;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != OfferStatus.Pending)
            throw new InvalidOperationException("Sadece bekleyen teklifler reddedilebilir.");

        Status = OfferStatus.Rejected;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != OfferStatus.Pending)
            throw new InvalidOperationException("Sadece bekleyen teklifler iptal edilebilir.");

        Status = OfferStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}