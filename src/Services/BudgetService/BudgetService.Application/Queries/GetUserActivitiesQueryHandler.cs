using BudgetService.Application.Interfaces;
using BudgetService.Domain;
using MediatR;

namespace BudgetService.Application.Queries;

public class GetUserActivitiesQueryHandler : IRequestHandler<GetUserActivitiesQuery, List<BudgetActivity>>
{
    private readonly IBudgetActivityRepository _activityRepository;

    public GetUserActivitiesQueryHandler(IBudgetActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<List<BudgetActivity>> Handle(GetUserActivitiesQuery request, CancellationToken cancellationToken)
        => await _activityRepository.GetByUserIdAsync(request.UserId, request.Type);
}