using BudgetService.Application.DTOs;
using BudgetService.Application.Interfaces;
using BudgetService.Domain;
using MediatR;

namespace BudgetService.Application.Queries;

public class GetBudgetSummaryQueryHandler : IRequestHandler<GetBudgetSummaryQuery, BudgetSummaryDto>
{
    private readonly IBudgetActivityRepository _activityRepository;
    private readonly ICategoryLookupService _categoryLookupService;

    public GetBudgetSummaryQueryHandler(
        IBudgetActivityRepository activityRepository,
        ICategoryLookupService categoryLookupService)
    {
        _activityRepository = activityRepository;
        _categoryLookupService = categoryLookupService;
    }

    public async Task<BudgetSummaryDto> Handle(GetBudgetSummaryQuery request, CancellationToken cancellationToken)
    {
        var activities = await _activityRepository.GetByUserIdAndMonthAsync(request.UserId, request.Year, request.Month);

        // InventoryService'e ulaşılamazsa boş dictionary döner - kategori isimleri null kalır,
        // ama özetin geri kalanı yine de hesaplanıp döndürülür.
        var categoryNames = await _categoryLookupService.GetCategoryNamesAsync();

        var summary = new BudgetSummaryDto
        {
            Year = request.Year,
            Month = request.Month,
            TotalSales = activities.Where(a => a.Type == ActivityType.Sale).Sum(a => a.Amount),
            TotalManualExpenses = activities.Where(a => a.Type == ActivityType.ManualExpense).Sum(a => a.Amount),
            SalesCount = activities.Count(a => a.Type == ActivityType.Sale),
            DonationCount = activities.Count(a => a.Type == ActivityType.Donation),
            ManualExpenseCount = activities.Count(a => a.Type == ActivityType.ManualExpense),
            TopPlatform = activities
                .Where(a => a.Platform is not null)
                .GroupBy(a => a.Platform)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault()
        };

        summary.NetActivity = summary.TotalSales - summary.TotalManualExpenses;

        summary.CategoryBreakdown = activities
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

        return summary;
    }
}