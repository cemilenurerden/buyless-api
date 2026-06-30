using BudgetService.Application.Common;
using BudgetService.Application.DTOs;
using BudgetService.Application.Interfaces;
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

        var stats = BudgetStatsCalculator.Calculate(activities, categoryNames);

        return new BudgetSummaryDto
        {
            Year = request.Year,
            Month = request.Month,
            TotalSales = stats.totalSales,
            TotalManualExpenses = stats.totalManualExpenses,
            NetActivity = stats.netActivity,
            SalesCount = stats.salesCount,
            DonationCount = stats.donationCount,
            ManualExpenseCount = stats.manualExpenseCount,
            TopPlatform = stats.topPlatform,
            CategoryBreakdown = stats.categoryBreakdown
        };
    }
}