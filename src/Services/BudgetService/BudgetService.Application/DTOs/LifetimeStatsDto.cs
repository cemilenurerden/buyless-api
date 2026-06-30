namespace BudgetService.Application.DTOs;

public class LifetimeStatsDto
{
    public decimal TotalSales { get; set; }
    public decimal TotalManualExpenses { get; set; }
    public decimal NetActivity { get; set; }
    public int SalesCount { get; set; }
    public int DonationCount { get; set; }
    public int ManualExpenseCount { get; set; }
    public string? TopPlatform { get; set; }
    public List<CategoryBreakdownDto> CategoryBreakdown { get; set; } = new();
}
