using BuyLess.Shared;

namespace MarketplaceService.Domain;

public class Listing
{
    public Guid Id { get; private set; }
    public Guid SellerId { get; private set; }
    public Guid? ItemId { get; private set; }          // Opsiyonel - InventoryService'teki Item'a referans
    public string Title { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public ItemCondition Condition { get; private set; }
    public ListingStatus Status { get; private set; } = ListingStatus.Active;
    public int ViewCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<ListingImage> _images = new();
    public IReadOnlyCollection<ListingImage> Images => _images.AsReadOnly();
    public List<Order> Orders { get; private set; } = new();
    public List<Offer> Offers { get; private set; } = new();

    private Listing() { }

    public Listing(
        Guid sellerId,
        string title,
        decimal price,
        ItemCondition condition,
        Guid? itemId = null,
        string? description = null,
        string currency = "TRY")
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("İlan başlığı boş olamaz.", nameof(title));

        if (price <= 0)
            throw new ArgumentException("Fiyat sıfırdan büyük olmalıdır.", nameof(price));

        Id = Guid.NewGuid();
        SellerId = sellerId;
        ItemId = itemId;
        Title = title;
        Description = description;
        Price = price;
        Currency = currency;
        Condition = condition;
        Status = ListingStatus.Active;
        ViewCount = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string title, string? description, decimal price, ItemCondition condition)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("İlan başlığı boş olamaz.", nameof(title));

        if (price <= 0)
            throw new ArgumentException("Fiyat sıfırdan büyük olmalıdır.", nameof(price));

        if (Status != ListingStatus.Active)
            throw new InvalidOperationException("Sadece aktif ilanlar güncellenebilir.");

        Title = title;
        Description = description;
        Price = price;
        Condition = condition;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddImage(string imageUrl, int displayOrder)
    {
        _images.Add(new ListingImage(Id, imageUrl, displayOrder));
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void MarkAsSold()
    {
        if (Status != ListingStatus.Active)
            throw new InvalidOperationException("Sadece aktif ilanlar satıldı olarak işaretlenebilir.");

        Status = ListingStatus.Sold;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status != ListingStatus.Active)
            throw new InvalidOperationException("Sadece aktif ilanlar iptal edilebilir.");

        Status = ListingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}