using InventoryService.Application.Interfaces;
using MediatR;

namespace InventoryService.Application.Commands;

public class SellItemCommandHandler : IRequestHandler<SellItemCommand>
{
    private readonly IItemRepository _itemRepository;
    private readonly IBudgetServiceClient _budgetServiceClient;

    public SellItemCommandHandler(IItemRepository itemRepository, IBudgetServiceClient budgetServiceClient)
    {
        _itemRepository = itemRepository;
        _budgetServiceClient = budgetServiceClient;
    }

    public async Task Handle(SellItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(request.Id);

        if (item is null || item.UserId != request.UserId)
            throw new InvalidOperationException("Belirtilen eşya bulunamadı veya size ait değil.");

        item.MarkAsSold();

        await _itemRepository.UpdateAsync(item);

        // BudgetService'e bildirim - bu başarısız olsa da satış işlemi zaten tamamlandı,
        // BudgetServiceClient hatayı kendi içinde yutup loglar.
        await _budgetServiceClient.NotifySaleAsync(item.Id, request.Amount, item.CategoryId, request.AccessToken);
    }
}