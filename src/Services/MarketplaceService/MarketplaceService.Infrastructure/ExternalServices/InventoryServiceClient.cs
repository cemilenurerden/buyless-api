using System.Net.Http.Headers;
using System.Net.Http.Json;
using MarketplaceService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace MarketplaceService.Infrastructure.ExternalServices;

public class InventoryServiceClient : IInventoryServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<InventoryServiceClient> _logger;

    public InventoryServiceClient(HttpClient httpClient, ILogger<InventoryServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task NotifySoldAsync(Guid itemId, decimal amount, string accessToken)
    {
        try
        {
            var payload = new { amount };

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/Item/{itemId}/sell")
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "InventoryService'e satış bildirimi başarısız. ItemId: {ItemId}, Status: {StatusCode}",
                    itemId, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "InventoryService'e satış bildirimi gönderilemedi. ItemId: {ItemId}", itemId);
        }
    }
}