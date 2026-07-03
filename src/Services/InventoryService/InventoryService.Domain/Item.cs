namespace InventoryService.Domain;
using BuyLess.Shared;

public class Item
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public int CategoryId { get; private set; }
    public int? BrandId { get; private set; }
    public string Name { get; private set; }
    public string? Color { get; private set; }
    public string? Size { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public DateTime PurchaseDate { get; private set; }
    public string? PurchaseSource { get; private set; }
    public string? Barcode { get; private set; }
    public string? ImageUrl { get; private set; }
    public ItemCondition Condition { get; private set; }
    public ItemStatus Status { get; private set; }
    public DateTime? LastWornDate { get; private set; }
    public int WornCount { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Item() { }

    public Item(
        Guid userId,
        int categoryId,
        string name,
        decimal purchasePrice,
        DateTime purchaseDate,
        int? brandId = null,
        string? color = null,
        string? size = null,
        string? purchaseSource = null,
        string? barcode = null,
        string? imageUrl = null,
        ItemCondition condition = ItemCondition.New,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Eşya adı boş olamaz.", nameof(name));

        if (purchasePrice < 0)
            throw new ArgumentException("Satın alma fiyatı negatif olamaz.", nameof(purchasePrice));

        Id = Guid.NewGuid();
        UserId = userId;
        CategoryId = categoryId;
        BrandId = brandId;
        Name = name;
        Color = color;
        Size = size;
        PurchasePrice = purchasePrice;
        PurchaseDate = purchaseDate;
        PurchaseSource = purchaseSource;
        Barcode = barcode;
        ImageUrl = imageUrl;
        Condition = condition;
        Status = ItemStatus.Active;
        WornCount = 0;
        Notes = notes;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordWear(DateTime wornDate)
    {
        if (Status != ItemStatus.Active)
            throw new InvalidOperationException("Aktif olmayan bir eşya için giyilme kaydı oluşturulamaz.");

        WornCount += 1;

        if (LastWornDate is null || wornDate > LastWornDate)
            LastWornDate = wornDate;

        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        string? color,
        string? size,
        string? imageUrl,
        ItemCondition condition,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Eşya adı boş olamaz.", nameof(name));

        Name = name;
        Color = color;
        Size = size;
        ImageUrl = imageUrl;
        Condition = condition;
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Donate()
    {
        EnsureActive();
        Status = ItemStatus.Donated;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsSold()
    {
        EnsureActive();
        Status = ItemStatus.Sold;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsRecycled()
    {
        EnsureActive();
        Status = ItemStatus.Recycled;
        UpdatedAt = DateTime.UtcNow;
    }

    private void EnsureActive()
    {
        if (Status != ItemStatus.Active)
            throw new InvalidOperationException($"Bu eşya zaten '{Status}' durumunda, durumu tekrar değiştirilemez.");
    }
}