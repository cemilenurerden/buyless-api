using BudgetService.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace BudgetService.Infrastructure.ExternalServices;

public class CategoryLookupService : ICategoryLookupService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CategoryLookupService> _logger;

    public CategoryLookupService(HttpClient httpClient, ILogger<CategoryLookupService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Dictionary<int, string>> GetCategoryNamesAsync()
    {
        try
        {
            // InventoryService'teki GET /api/Category herkese açık (Authorize gerektirmiyor),
            // bu yüzden token iletmeye gerek yok.
            var categories = await _httpClient.GetFromJsonAsync<List<CategoryResponse>>("api/Category");

            return categories?.ToDictionary(c => c.Id, c => c.Name) ?? new Dictionary<int, string>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "InventoryService'ten kategori isimleri alınamadı.");
            return new Dictionary<int, string>();
        }
    }

    private class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
