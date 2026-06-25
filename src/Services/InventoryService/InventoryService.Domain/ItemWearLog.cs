namespace InventoryService.Domain;

public class ItemWearLog
{
    public Guid Id { get; private set; }
    public Guid ItemId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime WornDate { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ItemWearLog() { }

    public ItemWearLog(Guid itemId, Guid userId, DateTime wornDate)
    {
        Id = Guid.NewGuid();
        ItemId = itemId;
        UserId = userId;
        WornDate = wornDate;
        CreatedAt = DateTime.UtcNow;
    }
}