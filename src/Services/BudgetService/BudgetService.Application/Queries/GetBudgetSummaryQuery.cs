using BudgetService.Application.DTOs;
using MediatR;

namespace BudgetService.Application.Queries;

public class GetBudgetSummaryQuery : IRequest<BudgetSummaryDto>
{
    public Guid UserId { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
}