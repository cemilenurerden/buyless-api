namespace InventoryService.Domain;

public class BarcodeProductCache
{
    public string Barcode { get; private set; } = null!;
    public string ProductName { get; private set; } = null!;
    public string? BrandName { get; private set; }
    public string? CategoryHint { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTime LastFetchedAt { get; private set; }

    private BarcodeProductCache() { }

    public BarcodeProductCache(
        string barcode,
        string productName,
        string? brandName = null,
        string? categoryHint = null,
        string? imageUrl = null)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            throw new ArgumentException("Barkod boş olamaz.", nameof(barcode));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Ürün adı boş olamaz.", nameof(productName));

        Barcode = barcode;
        ProductName = productName;
        BrandName = brandName;
        CategoryHint = categoryHint;
        ImageUrl = imageUrl;
        LastFetchedAt = DateTime.UtcNow;
    }

    // İleride dış servisten tekrar veri çekildiğinde, cache'i güncellemek için.
    public void Refresh(string productName, string? brandName, string? categoryHint, string? imageUrl)
    {
        ProductName = productName;
        BrandName = brandName;
        CategoryHint = categoryHint;
        ImageUrl = imageUrl;
        LastFetchedAt = DateTime.UtcNow;
    }
}