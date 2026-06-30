using BudgetService.Application.Interfaces;
using BudgetService.Domain;
using MediatR;

namespace BudgetService.Application.Commands;

public class DeleteManualExpenseCommandHandler : IRequestHandler<DeleteManualExpenseCommand>
{
    private readonly IBudgetActivityRepository _activityRepository;

    public DeleteManualExpenseCommandHandler(IBudgetActivityRepository activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task Handle(DeleteManualExpenseCommand request, CancellationToken cancellationToken)
    {
        var activity = await _activityRepository.GetByIdAsync(request.Id);

        if (activity is null || activity.UserId != request.UserId)
            throw new InvalidOperationException("Belirtilen hareket bulunamadı veya size ait değil.");

        // Sale ve Donation kayıtları InventoryService'ten otomatik geldiği için
        // BudgetService üzerinden silinemez - kaynağıyla tutarsızlık yaratır.
        if (activity.Type != ActivityType.ManualExpense)
            throw new InvalidOperationException("Sadece manuel olarak eklenen harcamalar silinebilir.");

        await _activityRepository.DeleteAsync(activity);
    }
}