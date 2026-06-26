using InventoryService.Application.Interfaces;
using MediatR;

namespace InventoryService.Application.Commands;

public class SellItemCommandHandler : IRequestHandler<SellItemCommand>
{
    private readonly IItemRepository _itemRepository;

    public SellItemCommandHandler(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task Handle(SellItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _itemRepository.GetByIdAsync(request.Id);

        if (item is null || item.UserId != request.UserId)
            throw new InvalidOperationException("Belirtilen eşya bulunamadı veya size ait değil.");

        item.MarkAsSold();

        await _itemRepository.UpdateAsync(item);
    }
}