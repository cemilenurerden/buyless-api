namespace BudgetService.Application.DTOs;

public class BudgetSummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalManualExpenses { get; set; }
    public decimal NetActivity { get; set; }
    public int SalesCount { get; set; }
    public int DonationCount { get; set; }
    public int ManualExpenseCount { get; set; }
    public string? TopPlatform { get; set; }
    public List<CategoryBreakdownDto> CategoryBreakdown { get; set; } = new();
}

public class CategoryBreakdownDto
{
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalEarned { get; set; }
}