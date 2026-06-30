using BudgetService.Application.Interfaces;
using BudgetService.Domain;
using BudgetService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BudgetService.Infrastructure.Repositories;

public class BudgetActivityRepository : IBudgetActivityRepository
{
    private readonly AppDbContext _context;

    public BudgetActivityRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BudgetActivity?> GetByIdAsync(Guid id)
        => await _context.BudgetActivities.FirstOrDefaultAsync(a => a.Id == id);

    public async Task<List<BudgetActivity>> GetByUserIdAsync(Guid userId, ActivityType? type = null)
    {
        var query = _context.BudgetActivities.Where(a => a.UserId == userId);

        if (type.HasValue)
            query = query.Where(a => a.Type == type.Value);

        return await query.OrderByDescending(a => a.OccurredAt).ToListAsync();
    }

    public async Task<List<BudgetActivity>> GetByUserIdAndMonthAsync(Guid userId, int year, int month)
        => await _context.BudgetActivities
            .Where(a => a.UserId == userId
                        && a.OccurredAt.Year == year
                        && a.OccurredAt.Month == month)
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync();

    public async Task<List<BudgetActivity>> GetByUserIdAndCategoryAsync(Guid userId, int categoryId)
        => await _context.BudgetActivities
            .Where(a => a.UserId == userId && a.CategoryId == categoryId)
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync();

    public async Task AddAsync(BudgetActivity activity)
    {
        await _context.BudgetActivities.AddAsync(activity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BudgetActivity activity)
    {
        _context.BudgetActivities.Remove(activity);
        await _context.SaveChangesAsync();
    }
}