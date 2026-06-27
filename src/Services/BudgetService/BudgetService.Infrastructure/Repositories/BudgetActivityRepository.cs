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

    public async Task<List<BudgetActivity>> GetByUserIdAsync(Guid userId)
        => await _context.BudgetActivities
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.OccurredAt)
            .ToListAsync();

    public async Task AddAsync(BudgetActivity activity)
    {
        await _context.BudgetActivities.AddAsync(activity);
        await _context.SaveChangesAsync();
    }
}