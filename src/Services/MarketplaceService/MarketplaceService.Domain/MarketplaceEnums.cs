namespace MarketplaceService.Domain;

public enum ListingStatus
{
    Active,
    Sold,
    Cancelled,
    Suspended
}

public enum OfferStatus
{
    Pending,
    Accepted,
    Rejected,
    Cancelled
}

public enum OrderStatus
{
    Pending,
    Paid,
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}

public enum PaymentStatus
{
    Pending,
    Success,
    Failed,
    Refunded
}

public enum PaymentMethod
{
    CreditCard,
    DebitCard
}