using BudgetService.Application.Common;
using BudgetService.Application.DTOs;
using BudgetService.Application.Interfaces;
using MediatR;

namespace BudgetService.Application.Queries;

public class GetLifetimeStatsQueryHandler : IRequestHandler<GetLifetimeStatsQuery, LifetimeStatsDto>
{
    private readonly IBudgetActivityRepository _activityRepository;
    private readonly ICategoryLookupService _categoryLookupService;

    public GetLifetimeStatsQueryHandler(
        IBudgetActivityRepository activityRepository,
        ICategoryLookupService categoryLookupService)
    {
        _activityRepository = activityRepository;
        _categoryLookupService = categoryLookupService;
    }

    public async Task<LifetimeStatsDto> Handle(GetLifetimeStatsQuery request, CancellationToken cancellationToken)
    {
        var activities = await _activityRepository.GetByUserIdAsync(request.UserId);

        var categoryNames = await _categoryLookupService.GetCategoryNamesAsync();

        var stats = BudgetStatsCalculator.Calculate(activities, categoryNames);

        return new LifetimeStatsDto
        {
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