using InventoryService.Application.Interfaces;
using MediatR;

namespace InventoryService.Application.Commands;

public class DonateItemCommandHandler : IRequestHandler<DonateItemCommand>
{
    private readonly IItemRepository _itemRepository;
    private readonly IBudgetServiceClient _budgetServiceClient;

    public DonateItemCommandHandler(IItemRepository itemRepository, IBudgetServiceClient budgetServiceClient)
    {
        _itemRepository = itemRepository;
        _budgetServiceClient = budgetServiceClient;
    }

    public async Task Handle(DonateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(request.Id);

        if (item is null || item.UserId != request.UserId)
            throw new InvalidOperationException("Belirtilen eşya bulunamadı veya size ait değil.");

        // Domain metodu: zaten Active değilse (Donated/Sold/Recycled) hata fırlatır.
        item.Donate();

        await _itemRepository.UpdateAsync(item);

        await _budgetServiceClient.NotifyDonationAsync(item.Id, item.CategoryId, request.AccessToken);
    }
}