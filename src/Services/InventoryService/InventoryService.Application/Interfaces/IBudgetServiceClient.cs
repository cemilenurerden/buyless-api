namespace InventoryService.Application.Interfaces;

public interface IBudgetServiceClient
{
    // accessToken: isteği yapan kullanıcının kendi JWT token'ı.
    // BudgetService bu token'dan UserId'yi okuyacağı için, InventoryService
    // ekstra bir UserId parametresi göndermek yerine token'ı aynen iletir.
    Task NotifySaleAsync(Guid itemId, decimal amount, int? categoryId, string accessToken);

    Task NotifyDonationAsync(Guid itemId, int? categoryId, string accessToken);
}