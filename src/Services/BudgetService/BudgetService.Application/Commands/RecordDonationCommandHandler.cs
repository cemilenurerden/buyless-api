using BudgetService.Application.Interfaces;
using BudgetService.Domain;
using MediatR;

namespace BudgetService.Application.Commands;

public class RecordDonationCommandHandler : IRequestHandler<RecordDonationCommand, Guid>
{
    private readonly IBudgetActivityRepository _activityRepository;

    public RecordDonationCommandHandler(IBudgetActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<Guid> Handle(RecordDonationCommand request, CancellationToken cancellationToken)
    {
        var activity = BudgetActivity.CreateFromDonation(
            request.UserId,
            request.ItemId,
            occurredAt: DateOnly.FromDateTime(DateTime.UtcNow),
            request.CategoryId);

        await _activityRepository.AddAsync(activity);

        return activity.Id;
    }
}
