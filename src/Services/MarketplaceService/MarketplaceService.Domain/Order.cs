namespace MarketplaceService.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public Guid ListingId { get; private set; }
    public Guid BuyerId { get; private set; }
    public Guid SellerId { get; private set; }
    public decimal Amount { get; private set; }
    public string Currency { get; private set; } = "TRY";
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public string? ShippingAddress { get; private set; }
    public string? TrackingNumber { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Order() { }

    public Order(Guid listingId, Guid buyerId, Guid sellerId, decimal amount, string currency = "TRY")
    {
        if (amount <= 0)
            throw new ArgumentException("Sipariş tutarı sıfırdan büyük olmalıdır.", nameof(amount));

        Id = Guid.NewGuid();
        ListingId = listingId;
        BuyerId = buyerId;
        SellerId = sellerId;
        Amount = amount;
        Currency = currency;
        Status = OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetShippingAddress(string shippingAddress)
    {
        ShippingAddress = shippingAddress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsShipped(string trackingNumber)
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException("Sadece ödemesi tamamlanmış siparişler kargoya verilebilir.");

        TrackingNumber = trackingNumber;
        Status = OrderStatus.Shipped;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Sadece kargodaki siparişler teslim edildi olarak işaretlenebilir.");

        Status = OrderStatus.Delivered;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsPaid()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Sadece bekleyen siparişler ödendi olarak işaretlenebilir.");

        Status = OrderStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            throw new InvalidOperationException("Kargoya verilmiş veya teslim edilmiş siparişler iptal edilemez.");

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}