namespace BudgetService.Application.Interfaces;

public interface ICategoryLookupService
{
    // InventoryService'ten kategori isimlerini sorar. Tek bir HTTP isteğiyle
    // tüm kategorileri çekip Id -> Name eşleşmesi döndürür.
    // InventoryService'e ulaşılamazsa boş bir dictionary döner (exception fırlatmaz) -
    // çağıran taraf, eksik isimleri null olarak ele alır.
    Task<Dictionary<int, string>> GetCategoryNamesAsync();
}
