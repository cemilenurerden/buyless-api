namespace MarketplaceService.Domain;

public class ListingImage
{
    public Guid Id { get; private set; }
    public Guid ListingId { get; private set; }
    public string ImageUrl { get; private set; } = null!;
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // EF Core navigation property
    public Listing Listing { get; private set; } = null!;

    private ListingImage() { }

    public ListingImage(Guid listingId, string imageUrl, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            throw new ArgumentException("Fotoğraf URL'i boş olamaz.", nameof(imageUrl));

        Id = Guid.NewGuid();
        ListingId = listingId;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        CreatedAt = DateTime.UtcNow;
    }
}