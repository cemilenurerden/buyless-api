namespace BudgetService.Domain;

public class BudgetActivity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? ItemId { get; private set; }
    public int? CategoryId { get; private set; }
    public ActivityType Type { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public DateOnly OccurredAt { get; private set; }
    public string? Platform { get; private set; }
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private BudgetActivity() { }

    private BudgetActivity(
        Guid userId,
        ActivityType type,
        decimal amount,
        DateOnly occurredAt,
        Guid? itemId,
        int? categoryId,
        string currency,
        string? platform,
        string? description)
    {
        if (amount < 0)
            throw new ArgumentException("Tutar negatif olamaz.", nameof(amount));

        Id = Guid.NewGuid();
        UserId = userId;
        ItemId = itemId;
        CategoryId = categoryId;
        Type = type;
        Amount = amount;
        Currency = string.IsNullOrWhiteSpace(currency) ? "TRY" : currency;
        OccurredAt = occurredAt;
        Platform = platform;
        Description = description;
        CreatedAt = DateTime.UtcNow;
    }

    // Kullanıcının elle girdiği harcama - genelde bir Item'a bağlı olur ama zorunlu değil.
    public static BudgetActivity CreateManualExpense(
        Guid userId,
        decimal amount,
        DateOnly occurredAt,
        Guid? itemId = null,
        int? categoryId = null,
        string currency = "TRY",
        string? platform = null,
        string? description = null)
    {
        return new BudgetActivity(userId, ActivityType.ManualExpense, amount, occurredAt, itemId, categoryId, currency, platform, description);
    }

    // InventoryService'ten otomatik gelir - bir eşya satıldığında.
    public static BudgetActivity CreateFromSale(
        Guid userId,
        Guid itemId,
        decimal amount,
        DateOnly occurredAt,
        int? categoryId = null,
        string currency = "TRY")
    {
        return new BudgetActivity(userId, ActivityType.Sale, amount, occurredAt, itemId, categoryId, currency, platform: null, description: null);
    }

    // InventoryService'ten otomatik gelir - bir eşya bağışlandığında.
    // Bağışta parasal bir tutar olmadığı için Amount 0 olarak kaydedilir;
    // bu kaydın varlığı zaten "X eşyayı bağışladın" bilgisini taşıyor.
    public static BudgetActivity CreateFromDonation(
        Guid userId,
        Guid itemId,
        DateOnly occurredAt,
        int? categoryId = null)
    {
        return new BudgetActivity(userId, ActivityType.Donation, amount: 0, occurredAt, itemId, categoryId, currency: "TRY", platform: null, description: null);
    }
}