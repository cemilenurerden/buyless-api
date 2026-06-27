using BudgetService.Domain;

namespace BudgetService.Application.Interfaces;

public interface IBudgetActivityRepository
{
    Task<BudgetActivity?> GetByIdAsync(Guid id);
    Task<List<BudgetActivity>> GetByUserIdAsync(Guid userId);
    Task AddAsync(BudgetActivity activity);
}
