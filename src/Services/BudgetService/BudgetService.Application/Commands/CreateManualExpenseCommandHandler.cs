using BudgetService.Application.Interfaces;
using BudgetService.Domain;
using MediatR;

namespace BudgetService.Application.Commands;

public class CreateManualExpenseCommandHandler : IRequestHandler<CreateManualExpenseCommand, Guid>
{
    private readonly IBudgetActivityRepository _activityRepository;

    public CreateManualExpenseCommandHandler(IBudgetActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<Guid> Handle(CreateManualExpenseCommand request, CancellationToken cancellationToken)
    {
        var activity = BudgetActivity.CreateManualExpense(
            request.UserId,
            request.Amount,
            request.OccurredAt,
            request.ItemId,
            request.CategoryId,
            request.Currency,
            request.Platform,
            request.Description);

        await _activityRepository.AddAsync(activity);

        return activity.Id;
    }
}