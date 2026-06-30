using BudgetService.Application.DTOs;
using MediatR;

namespace BudgetService.Application.Queries;

public class GetLifetimeStatsQuery : IRequest<LifetimeStatsDto>
{
    public Guid UserId { get; set; }
}