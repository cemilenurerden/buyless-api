namespace MarketplaceService.Application.Interfaces;

public interface IInventoryServiceClient
{
    /// <summary>
    /// Bir Item satıldığında InventoryService'e bildirim gönderir.
    /// Hata olursa sessizce loglar, satış işlemini engellemez.
    /// </summary>
    Task NotifySoldAsync(Guid itemId, decimal amount, string accessToken);
}