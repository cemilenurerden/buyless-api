using System.Net.Http.Headers;
using System.Net.Http.Json;
using InventoryService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.ExternalServices;

public class BudgetServiceClient : IBudgetServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BudgetServiceClient> _logger;

    public BudgetServiceClient(HttpClient httpClient, ILogger<BudgetServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task NotifySaleAsync(Guid itemId, decimal amount, int? categoryId, string accessToken)
    {
        var payload = new
        {
            itemId,
            amount,
            categoryId,
            currency = "TRY"
        };

        await PostSilentlyAsync("api/Budget/activities/sale", payload, accessToken, "satış");
    }

    public async Task NotifyDonationAsync(Guid itemId, int? categoryId, string accessToken)
    {
        var payload = new
        {
            itemId,
            categoryId
        };

        await PostSilentlyAsync("api/Budget/activities/donation", payload, accessToken, "bağış");
    }

    // BudgetService'e ulaşılamazsa (servis kapalı, timeout, network hatası vb.) bu metod
    // exception fırlatmaz - sadece loglar. Böylece BudgetService'teki bir sorun,
    // InventoryService'teki asıl işlemi (satış/bağış) engellemez.
    private async Task PostSilentlyAsync(string path, object payload, string accessToken, string activityLabel)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = JsonContent.Create(payload)
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "BudgetService'e {Activity} bildirimi başarısız oldu. Status: {StatusCode}",
                    activityLabel, response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "BudgetService'e {Activity} bildirimi gönderilemedi.", activityLabel);
        }
    }
}