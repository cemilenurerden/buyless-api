using BudgetService.Application.DTOs;
using BudgetService.Domain;

namespace BudgetService.Application.Common;

// Hem aylık özet (GetBudgetSummaryQueryHandler) hem de tüm-zamanlar istatistiği
// (GetLifetimeStatsQueryHandler) aynı hesaplama mantığını kullanıyor; kod tekrarını
// önlemek için bu ortak metoda çıkarıldı.
public static class BudgetStatsCalculator
{
    public static (decimal totalSales, decimal totalManualExpenses, decimal netActivity,
        int salesCount, int donationCount, int manualExpenseCount, string? topPlatform,
        List<CategoryBreakdownDto> categoryBreakdown) Calculate(
            List<BudgetActivity> activities,
            Dictionary<int, string> categoryNames)
    {
        var totalSales = activities.Where(a => a.Type == ActivityType.Sale).Sum(a => a.Amount);
        var totalManualExpenses = activities.Where(a => a.Type == ActivityType.ManualExpense).Sum(a => a.Amount);

        var topPlatform = activities
            .Where(a => a.Platform is not null)
            .GroupBy(a => a.Platform)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault();

        var categoryBreakdown = activities
            .Where(a => a.CategoryId.HasValue)
            .GroupBy(a => a.CategoryId!.Value)
            .Select(g => new CategoryBreakdownDto
            {
                CategoryId = g.Key,
                CategoryName = categoryNames.TryGetValue(g.Key, out var name) ? name : null,
                TotalSpent = g.Where(a => a.Type == ActivityType.ManualExpense).Sum(a => a.Amount),
                TotalEarned = g.Where(a => a.Type == ActivityType.Sale).Sum(a => a.Amount)
            })
            .ToList();

        return (
            totalSales,
            totalManualExpenses,
            totalSales - totalManualExpenses,
            activities.Count(a => a.Type == ActivityType.Sale),
            activities.Count(a => a.Type == ActivityType.Donation),
            activities.Count(a => a.Type == ActivityType.ManualExpense),
            topPlatform,
            categoryBreakdown);
    }
}