namespace MarketplaceService.Domain;

public class Message
{
    public Guid Id { get; private set; }
    public Guid ListingId { get; private set; }
    public Guid SenderId { get; private set; }
    public Guid ReceiverId { get; private set; }
    public string Content { get; private set; } = null!;
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; private set; }

    // Navigation property
    public Listing Listing { get; private set; } = null!;

    private Message() { }

    public Message(Guid listingId, Guid senderId, Guid receiverId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Mesaj içeriği boş olamaz.", nameof(content));

        if (senderId == receiverId)
            throw new InvalidOperationException("Kendinize mesaj gönderemezsiniz.");

        Id = Guid.NewGuid();
        ListingId = listingId;
        SenderId = senderId;
        ReceiverId = receiverId;
        Content = content;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
