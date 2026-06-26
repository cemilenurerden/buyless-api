namespace BudgetService.Domain;

public enum ActivityType
{
    Sale,           // Eşya satıldı (InventoryService'ten otomatik gelir)
    Donation,       // Eşya bağışlandı (InventoryService'ten otomatik gelir)
    ManualExpense   // Kullanıcının elle girdiği harcama
}
