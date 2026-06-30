using BudgetService.Domain;

namespace BudgetService.Application.Interfaces;

public interface IBudgetActivityRepository
{
    Task<BudgetActivity?> GetByIdAsync(Guid id);
    Task<List<BudgetActivity>> GetByUserIdAsync(Guid userId, ActivityType? type = null);
    Task<List<BudgetActivity>> GetByUserIdAndMonthAsync(Guid userId, int year, int month);
    Task<List<BudgetActivity>> GetByUserIdAndCategoryAsync(Guid userId, int categoryId);
    Task AddAsync(BudgetActivity activity);
    Task DeleteAsync(BudgetActivity activity);
}