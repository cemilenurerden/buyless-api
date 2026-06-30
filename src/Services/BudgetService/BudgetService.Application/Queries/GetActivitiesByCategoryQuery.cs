using BudgetService.Domain;
using MediatR;

namespace BudgetService.Application.Queries;

public class GetActivitiesByCategoryQuery : IRequest<List<BudgetActivity>>
{
    public Guid UserId { get; set; }
    public int CategoryId { get; set; }
}