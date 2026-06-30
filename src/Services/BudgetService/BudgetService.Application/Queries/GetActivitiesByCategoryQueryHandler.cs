using BudgetService.Application.Interfaces;
using BudgetService.Domain;
using MediatR;

namespace BudgetService.Application.Queries;

public class GetActivitiesByCategoryQueryHandler : IRequestHandler<GetActivitiesByCategoryQuery, List<BudgetActivity>>
{
    private readonly IBudgetActivityRepository _activityRepository;

    public GetActivitiesByCategoryQueryHandler(IBudgetActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<List<BudgetActivity>> Handle(GetActivitiesByCategoryQuery request, CancellationToken cancellationToken)
        => await _activityRepository.GetByUserIdAndCategoryAsync(request.UserId, request.CategoryId);
}