using BudgetService.Application.Interfaces;
using BudgetService.Domain;
using MediatR;

namespace BudgetService.Application.Commands;

public class RecordSaleCommandHandler : IRequestHandler<RecordSaleCommand, Guid>
{
    private readonly IBudgetActivityRepository _activityRepository;

    public RecordSaleCommandHandler(IBudgetActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<Guid> Handle(RecordSaleCommand request, CancellationToken cancellationToken)
    {
        var activity = BudgetActivity.CreateFromSale(
            request.UserId,
            request.ItemId,
            request.Amount,
            occurredAt: DateOnly.FromDateTime(DateTime.UtcNow),
            request.CategoryId,
            request.Currency);

        await _activityRepository.AddAsync(activity);

        return activity.Id;
    }
}