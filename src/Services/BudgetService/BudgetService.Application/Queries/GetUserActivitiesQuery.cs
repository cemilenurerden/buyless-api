using BudgetService.Domain;
using MediatR;

namespace BudgetService.Application.Queries;

public class GetUserActivitiesQuery : IRequest<List<BudgetActivity>>
{
    public Guid UserId { get; set; }
}